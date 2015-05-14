using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

// Used to label the main room types
public enum RoomType
{
	STARTER,
	HORDE,
	PUZZLE,
	BOSS,
	TREASURE,
	REACTION
}

// Used to label the four main cardinal directions
public enum Direction
{
	NORTH,
	SOUTH,
	EAST,
	WEST,
	NONE
}

// This graph makes up all of the connected rooms in a dungeon
public class RoomGraph
{
	public List<RoomNode> rooms;
	public List<RoomNode> roomsActive;

	public RoomGraph()
	{
		rooms = new List<RoomNode>();
		roomsActive = new List<RoomNode>();
	}

	public void addRoom(string name)
	{
		RoomNode room = new RoomNode(name);
		rooms.Add(room);
	}

	public RoomNode getNodeByRoomName(string roomName)
	{
		for (int i = 0; i < rooms.Count; i++)
		{
			if (rooms[i].name == roomName)
			{
				return rooms[i];
			}
		}
		return null;
	}

	// Creates a neighboring connection in the graph structure between the two given rooms
	public void connectRooms(RoomNode roomOne, RoomNode roomTwo, Direction roomOneExit)
	{
		switch (roomOneExit)
		{
		case Direction.NORTH:
			roomOne.north = roomTwo;
			roomTwo.south = roomOne;
			break;
		case Direction.SOUTH:
			roomOne.south = roomTwo;
			roomTwo.north = roomOne;
			break;
		case Direction.EAST:
			roomOne.east = roomTwo;
			roomTwo.west = roomOne;
			break;
		case Direction.WEST:
			roomOne.west = roomTwo;
			roomTwo.east = roomOne;
			break;
		default:
			Debug.Log("Error connecting rooms " + roomOne.name + " and " + roomTwo.name + ": no direction");
			break;
		}
	}
}

// These nodes make up the contents of the RoomGraph. Each RoomNode represents a room in the graph
public class RoomNode
{
	public string name;
	public GameObject obj;
	public RoomType type;
	public bool hasBeenLoaded = false; // used to keep track of the first time a room is loaded for item management
	public List<GameObject> doors;
	public List<GameObject> items;
	public List<GameObject> playerRespawns;
	public List<EnemySpawner> enemySpawners;
	public RoomNode[] neighbors;
	public RoomNode north
	{
		get
		{
			return neighbors[0];
		}
		set
		{
			neighbors[0] = value;
		}
	}
	public RoomNode east
	{
		get
		{
			return neighbors[1];
		}
		set
		{
			neighbors[1] = value;
		}
	}
	public RoomNode south
	{
		get
		{
			return neighbors[2];
		}
		set
		{
			neighbors[2] = value;
		}
	}
	public RoomNode west
	{
		get
		{
			return neighbors[3];
		}
		set
		{
			neighbors[3] = value;
		}
	}

	public RoomNode(string n)
	{
		name = n;
		neighbors = new RoomNode[4] {null, null, null, null};
		doors = new List<GameObject>();
		items = new List<GameObject>();
		playerRespawns = new List<GameObject>();
		enemySpawners = new List<EnemySpawner>();
		// set up room type
		if (n.Contains("_H_"))
		{
			type = RoomType.HORDE;
		}
		else if (n.Contains("_P_"))
		{
			type = RoomType.PUZZLE;
		}
		else if (n.Contains("_B_"))
		{
			type = RoomType.BOSS;
		}
		else if (n.Contains("_T_"))
		{
			type = RoomType.TREASURE;
		}
		else if (n.Contains("_R_"))
		{
			type = RoomType.REACTION;
		}
		else
		{
			type = RoomType.STARTER;
		}
	}
}

public class MapManager : MonoBehaviour 
{
	public struct RoomPrefab
	{
		public string prefabName;
		public int[] position;
		public Direction exit;
		public string type;

		public RoomPrefab(string name, int[] pos, Direction ex, string rt)
		{
			prefabName = name;
			position = pos;
			exit = ex;
			type = rt;
		}
	};

	// set to true for procedural map generation, false to use a preset order of rooms
	// note: preset order of rooms has to use the public roomsToLoad array and manually have the rooms
	// connected with map.connectRooms() after the map has been created
	public bool proceduralGeneration = false;	
	// (for procedural map generation) set to true to balance the types of rooms that are being loaded
	public bool roomBalance = true;
	// (for procedural map generation) set to true for every room in the dungeon to be unique
	public bool noRoomRepeats = false;

	private RoomGraph map;
	public int numRooms;							// the number of rooms used along the main path, not counting branches
	public string[] roomsToLoad = new string[3];	// temp array used for loading premade dungeons

	private int goalHordeRooms;
	private int goalPuzzleRooms;
	private int goalReactionRooms;

	private string RoomPrefabFilePath = "Assets/Resources/Prefabs/Environment/Resources/";
	DirectoryInfo dir;
	FileInfo[] info;

	private int playerSpawnRoom = 0;

	private PlayerManager pMan;

	void Awake()
	{
		// Set up the map and the rooms that will be used in the dungeon
		map = new RoomGraph();
		generateDungeon();
	}

	void Start()
	{
		foreach (GameObject player in GameObject.Find("PlayerManager").GetComponent<PlayerManager>().players)
		{
			player.GetComponent<PlayerBase>().roomIn = map.rooms[0];
		}
	}

	// Called on awake. Generates the dungeon and sets the players in the first room
	private void generateDungeon()
	{
		if (proceduralGeneration)
		{
			// Don't even try to generate the dungeon if we have less than 2 rooms
			if (numRooms < 2)
			{
				Debug.Log ("Error generating dungeon, too few numRooms specified");
				return;
			}
			// Figure out how many of each room we want for a balanced dungeon
			// (Half horde rooms, quarter puzzle rooms, quarter reaction rooms, favor puzzle rooms if odd number)
			if (roomBalance)
			{
				goalHordeRooms = Mathf.CeilToInt((float)(numRooms - 2) / 2.0f);
				goalPuzzleRooms = Mathf.CeilToInt((float)(numRooms - 2 - goalHordeRooms) / 2.0f);
				goalReactionRooms = numRooms - 2 - goalHordeRooms - goalPuzzleRooms;
			}

			// Get references to all of the room prefab files
			dir = new DirectoryInfo(RoomPrefabFilePath);
			info = dir.GetFiles("*.prefab");

			// Generate the dungeon with a recursive function that works one room at a time
			List<RoomPrefab> roomsToUse = new List<RoomPrefab>();
			Dictionary<string, int> numRoomTypes = new Dictionary<string, int>(){{"_H_", 0}, {"_P_", 0}, {"_R_", 0}, {"_T_", 0}, {"_S_", 0}, {"_B_", 0}};
			generateRoom(roomsToUse, numRoomTypes);

			// Log an error message if the generation failed
			if (roomsToUse.Count == 0)
			{
				Debug.Log ("Error generating dungeon, could not generate dungeon from given room prefabs");
				return;
			}

			// Once the main path through the dungeon has been generated, replace a few of the rooms with rooms that can branch
			List<RoomPrefab> sideRooms = setupSidePaths(roomsToUse);

			// Once all of the rooms have been selected, set up the map and connect the rooms
			foreach (RoomPrefab rp in roomsToUse)
			{
				string rName = rp.prefabName.Substring(0, rp.prefabName.Length - 7);
				map.addRoom(rName);
			}
			for (int i = 0; i < map.rooms.Count - 1; i++)
			{
				map.connectRooms(map.rooms[i], map.rooms[i+1], roomsToUse[i].exit);
			}
			// Load the first room and its neighbors
			loadRoom(map.rooms[0]);
			//loadNeighbors(map.rooms[0]);
			for (int i = 0; i < map.rooms.Count; i++)
			{
				loadNeighbors(map.rooms[i]);
			}
			// Give the player manager the first room's spawn points
			pMan = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
			pMan.assignNewSpawnPoints(map.rooms[0].playerRespawns.ToArray());
		}
		else
		{
			for (int i = 0; i < roomsToLoad.Length; i++)
			{
				map.addRoom(roomsToLoad[i]);
			}
			// Set up the first room of the dungeon
			loadRoom(map.rooms[0]);
			// hardcoded room neighbors, this must be done by hand to get rooms to connect for this method 
			map.connectRooms(map.rooms[0], map.rooms[1], Direction.NORTH);
			map.connectRooms(map.rooms[1], map.rooms[2], Direction.NORTH);
			map.connectRooms(map.rooms[2], map.rooms[3], Direction.WEST);
			// Look at the first room's neighbors and set them up
			loadNeighbors(map.rooms[0]);
			// Give the player manager the first room's spawn points
			pMan = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
			pMan.assignNewSpawnPoints(map.rooms[0].playerRespawns.ToArray());
		}
	}

	private bool generateRoom(List<RoomPrefab> rooms, Dictionary<string, int> numRoomTypes)
	{
		// Figure out which entrance the next room has to have to connect with the previous room
		string lookingFor = "";
		if (rooms.Count > 0)
		{
			switch (rooms[rooms.Count-1].exit)
			{
			case Direction.NORTH:
				lookingFor = "S";
				break;
			case Direction.WEST:
				lookingFor = "E";
				break;
			case Direction.EAST:
				lookingFor = "W";
				break;
			case Direction.SOUTH:
				lookingFor = "N";
				break;
			}
		}

		bool roomSet = false; 								// becomes true when the room is safe to use
		List<string> roomsNotToUse = new List<string>(); 	// a list of all the rooms that we have tried that did not work

		// Continue trying to pick a room until either a suitable one is found or all options are exhausted, upon which we scrap this room and back out to the previous one
		while (!roomSet)
		{
			// Look through all room prefabs and select all of the rooms that can connect with the previous one that haven't been used yet
			int[] nextRoomPos = new int[2];
			List<string> potentialRooms = new List<string>();
			foreach (FileInfo f in info)
			{
				if (roomsNotToUse.Contains(f.Name))
				{
					continue;
				}

				string fileName = f.Name.Substring(0, f.Name.Length - 7); // remove ".prefab"
				string e = fileName.Substring(f.Name.LastIndexOf("_") + 1); // get exit directions string

				// Make sure that this room won't overlap with any existing rooms
				if (rooms.Count > 0)
				{
					nextRoomPos[0] = rooms[rooms.Count-1].position[0];
					nextRoomPos[1] = rooms[rooms.Count-1].position[1];
					
					switch (rooms[rooms.Count-1].exit)
					{
					case Direction.NORTH:
						lookingFor = "S";
						nextRoomPos[1]++;
						break;
					case Direction.WEST:
						lookingFor = "E";
						nextRoomPos[0]--;
						break;
					case Direction.EAST:
						lookingFor = "W";
						nextRoomPos[0]++;
						break;
					case Direction.SOUTH:
						lookingFor = "N";
						nextRoomPos[1]--;
						break;
					}

					bool willOverlap = false;
					foreach (RoomPrefab rp in rooms)
					{
						if (rp.position[0] == nextRoomPos[0] && rp.position[1] == nextRoomPos[1])
						{
							willOverlap = true;
							break;
						}
					}
					if (willOverlap)
					{
						roomsNotToUse.Add(f.Name);
						continue;
					}
				}

				// First room
				if (rooms.Count == 0)
				{
					// first room must be starter type
					if (fileName.Contains("_S_"))
					{
						potentialRooms.Add(fileName);
					}
				}
				// Final room
				else if (rooms.Count == numRooms - 1)
				{
					// makes sure the final room is a boss room
					if (fileName.Contains("_B_") && e.Contains(lookingFor))
					{
						potentialRooms.Add(fileName);
					}
				}
				// Middle rooms
				else
				{
					// Room must have the required entrance to connect with the previous room and must have another exit in addition to that
					if (e.Contains(lookingFor) && e.Length == 2)
					{
						if (noRoomRepeats)	// don't include rooms we have used previously
						{
							bool foundRepeat = false;

							foreach (RoomPrefab rp in rooms)
							{
								if (rp.prefabName == f.Name)
								{
									foundRepeat = true;
									break;
								}
							}

							if (foundRepeat)
							{
								roomsNotToUse.Add(f.Name);
								continue;
							}
						}

						if (roomBalance)	// carry out various actions to balance the room selection
						{
							if (fileName.Contains(rooms[rooms.Count-1].type))	// don't do the same type of room twice in a row
							{
								roomsNotToUse.Add(f.Name);
								continue;
							}

							if (fileName.Contains("_H_") && numRoomTypes["_H_"] == goalHordeRooms)
							{
								roomsNotToUse.Add(f.Name);
								continue;
							}
							if (fileName.Contains("_P_") && numRoomTypes["_P_"] == goalPuzzleRooms)
							{
								roomsNotToUse.Add(f.Name);
								continue;
							}
							if (fileName.Contains("_R_") && numRoomTypes["_R_"] == goalReactionRooms)
							{
								roomsNotToUse.Add(f.Name);
								continue;
							}
						}

						// If a puzzle or reaction room, make sure that we are using the right entrance to the room
						// (the required entrance will be the first character in e)
						if (fileName.Contains("_P_") || fileName.Contains("_R_"))
						{
							if (e[0].ToString() != lookingFor)
							{
								roomsNotToUse.Add(f.Name);
								continue;
							}
						}

						// If we made it through the optional checks, then go ahead and add the room
						potentialRooms.Add(fileName);
					}
				}
			}
			// If no rooms work for branching from this one, then we have to scrap this one
			if (potentialRooms.Count == 0)
			{
				// If we have a room to go back to, scrap this room and continue generation from the previous one
				if (rooms.Count > 0)
				{
					rooms.RemoveAt(rooms.Count-1);
					return false;
				}
				// Otherwise, we are at the starter room, and generation has failed for all possible room combos, so we give up :(
				else
				{
					break;
				}
			}
			// Select one of these rooms at random and create the room
			int idx = Random.Range(0, potentialRooms.Count);
			string roomName = potentialRooms[idx];
			string thisExit = roomName.Substring(roomName.LastIndexOf("_") + 1);
			Direction exitDir = Direction.NONE;
			if (lookingFor != "")
			{
				thisExit = thisExit.Replace(lookingFor, "");	// currently only supports rooms with 2 entrances
			}
			switch (thisExit)
			{
			case "N":
				exitDir = Direction.NORTH;
				break;
			case "W":
				exitDir = Direction.WEST;
				break;
			case "E":
				exitDir = Direction.EAST;
				break;
			case "S":
				exitDir = Direction.SOUTH;
				break;
			}
			string type = "_S_";
			if (roomName.Contains("_H_"))
			{
				type = "_H_";
			}
			else if (roomName.Contains("_P_"))
			{
				type = "_P_";
			}
			else if (roomName.Contains("_B_"))
			{
				type = "_B_";
			}
			else if (roomName.Contains("_T_"))
			{
				type = "_T_";
			}
			else if (roomName.Contains("_R_"))
			{
				type = "_R_";
			}
			Dictionary<string, int> newNumRoomTypes = new Dictionary<string, int>(numRoomTypes);
			newNumRoomTypes[type]++;
			RoomPrefab newRoom = new RoomPrefab(roomName + ".prefab", nextRoomPos, exitDir, type);
			rooms.Add(newRoom);
			roomsNotToUse.Add(roomName + ".prefab");
			// If we have all rooms of the dungeon, return a success
			if (rooms.Count == numRooms)
			{
				return true;
			}
			// Set up generation of the next room if not done
			roomSet = generateRoom(rooms, newNumRoomTypes);
		}
		return true;
	}

	private List<RoomPrefab> setupSidePaths(List<RoomPrefab> rooms)
	{
		List<RoomPrefab> sideRooms = new List<RoomPrefab>();

		// Get a list of all rooms that can be used for branching
		List<string> potentialRooms = new List<string>();
		foreach (FileInfo f in info)
		{
			string fileName = f.Name.Substring(0, f.Name.Length - 7); 	// remove ".prefab"
			string e = fileName.Substring(f.Name.LastIndexOf("_") + 1); // get exit directions string
			
			if (e.Length >= 3)
			{
				potentialRooms.Add(fileName);
			}
		}
		
		// Pick a random room from the dungeon aside from the start and boss room to replace with a branching room
		int roomToBranch = Random.Range(1, rooms.Count - 1);
		int i = roomToBranch;
		do 
		{
			// Figure out which branching rooms could be used for replacing this room
			string pName = rooms[i].prefabName.Substring(0, rooms[i].prefabName.Length - 7);
			string se = pName.Substring(pName.LastIndexOf("_") + 1);		// room to replace entrances
			List<string> roomChoices = new List<string>();
			
			foreach (string roomName in potentialRooms)
			{
				string pe = roomName.Substring(roomName.LastIndexOf("_") + 1); 	// branching room entrances
				
				// Make sure that the branching room has the same two entrances that the room it's replacing does
				if (pe.Contains(se[0].ToString()) && pe.Contains(se[1].ToString()))
				{
					roomChoices.Add(roomName);
				}
			}

			bool success = false;
			string branchName = "";

			Debug.Log (rooms[i].position[0] + " " + rooms[i].position[1]);
			Debug.Log ("\n");
			
			foreach (string roomName in roomChoices)
			{
				string pe = roomName.Substring(roomName.LastIndexOf("_") + 1);
				pe = pe.Replace(se[0].ToString(), "");
				pe = pe.Replace(se[1].ToString(), "");
				
				foreach (char e in pe)
				{
					if (generateSidePaths(rooms, sideRooms, e.ToString(), rooms[i].position))
					{
						branchName = roomName + ".prefab";
						success = true;
						break;
					}
				}

				if (success)
				{
					break;
				}
			}

			if (success)
			{
				Debug.Log ("work");
				rooms[i] = new RoomPrefab(branchName, rooms[i].position, rooms[i].exit, rooms[i].type);
				break;
			}
			else
			{
				break;
			}
			
			// Continue on to the next room in the range of 1 thru rooms.Count - 2
			i = i == rooms.Count - 2 ? 1 : i + 1;
		} while (i != roomToBranch);

		// Take the created side path and add it to the dungeon


		return sideRooms;
	}

	private bool generateSidePaths(List<RoomPrefab> mainPath, List<RoomPrefab> sidePath, string lastExit, int[] position)
	{
		// Figure out which entrance the next room has to have to connect with the previous room
		bool roomSet = false; 								// becomes true when the room is safe to use
		List<string> roomsNotToUse = new List<string>(); 	// a list of all the rooms that we have tried that did not work

		if (noRoomRepeats)
		{
			foreach (RoomPrefab rp in mainPath)
			{
				roomsNotToUse.Add(rp.prefabName);
			}
		}

		string lookingFor = "";
		switch (lastExit)
		{
		case "N":
			lookingFor = "S";
			position[1]++;
			break;
		case "W":
			lookingFor = "E";
			position[0]--;
			break;
		case "E":
			lookingFor = "W";
			position[0]++;
			break;
		case "S":
			lookingFor = "N";
			position[1]--;
			break;
		}

		Debug.Log (position [0] + " " + position [1]);
		
		while (!roomSet)
		{
			// Look through all room prefabs and select all of the rooms that can connect with the previous one that haven't been used yet
			List<string> potentialRooms = new List<string>();
			foreach (FileInfo f in info)
			{
				if (roomsNotToUse.Contains(f.Name))
				{
					continue;
				}
				
				string fileName = f.Name.Substring(0, f.Name.Length - 7); // remove ".prefab"
				string e = fileName.Substring(f.Name.LastIndexOf("_") + 1); // get exit directions string
				
				bool willOverlap = false;
				foreach (RoomPrefab rp in mainPath)
				{
					if (rp.position[0] == position[0] && rp.position[1] == position[1])
					{
						willOverlap = true;
						break;
					}
				}
				if (willOverlap)
				{
					roomsNotToUse.Add(f.Name);
					continue;
				}

				if (sidePath.Count == 0 && !fileName.Contains("_H_"))
				{
					roomsNotToUse.Add(f.Name);
					continue;
				}
				else if (sidePath.Count == 1 && !fileName.Contains("_T_"))
				{
					roomsNotToUse.Add(f.Name);
					continue;
				}

				// If we got here, the room is good to use
				potentialRooms.Add(fileName);
			}

			// If no rooms work for branching from this one, then we have to scrap this one
			if (potentialRooms.Count == 0)
			{
				// If we have a room to go back to, scrap this room and continue generation from the previous one
				if (sidePath.Count > 0)
				{
					sidePath.RemoveAt(sidePath.Count-1);
					return false;
				}
				// Otherwise, we are at the starter room, and generation has failed for all possible room combos, so we give up :(
				else
				{
					break;
				}
			}
			else
			{
				Debug.Log ("found potential");
			}

			// Select one of these rooms at random and create the room
			int idx = Random.Range(0, potentialRooms.Count);
			string roomName = potentialRooms[idx];
			string thisExit = roomName.Substring(roomName.LastIndexOf("_") + 1);
			Direction exitDir = Direction.NONE;
			if (lookingFor != "")
			{
				thisExit = thisExit.Replace(lookingFor, "");
			}
			switch (thisExit)
			{
			case "N":
				exitDir = Direction.NORTH;
				break;
			case "W":
				exitDir = Direction.WEST;
				break;
			case "E":
				exitDir = Direction.EAST;
				break;
			case "S":
				exitDir = Direction.SOUTH;
				break;
			}
			string type = "_H_";
			if (sidePath.Count > 0)
			{
				type = "_T_";
			}
			RoomPrefab newRoom = new RoomPrefab(roomName + ".prefab", position, exitDir, type);
			sidePath.Add(newRoom);
			roomsNotToUse.Add(roomName + ".prefab");
			// If we have all rooms of the dungeon, return a success
			if (sidePath.Count == 2)
			{
				return true;
			}
			// Set up generation of the next room if not done

			roomSet = generateSidePaths(mainPath, sidePath, thisExit, position);
		}
		return false;
	}
	
	// Loads room and returns true if loading for first time
	private bool loadRoom(RoomNode roomNode)
	{
		if (!roomNode.hasBeenLoaded)
		{
			GameObject room = Instantiate(Resources.Load(roomNode.name), Vector3.zero, Quaternion.Euler(0.0f, 180.0f, 0.0f)) as GameObject;
			roomNode.obj = room;
			map.roomsActive.Add(roomNode);
			// Gather all of the room's respawn points
			GameObject[] respawns = GameObject.FindGameObjectsWithTag("Respawn");
			foreach (GameObject r in respawns)
			{
				if (r.transform.root == roomNode.obj.transform)
				{
					roomNode.playerRespawns.Add(r);
				}
			}
			EnemySpawner[] es = room.GetComponentsInChildren<EnemySpawner>();
			for (int i = 0; i < es.Length; i++)
			{
				roomNode.enemySpawners.Add(es[i]);
			}
			roomNode.hasBeenLoaded = true;
			return true;
		}
		else
		{
			roomNode.obj.SetActive(true);
			return false;
		}
	}

	// Looks at all of the neighbors of the given RoomNode and loads them up if they aren't already loaded. Also performs any extra room setup needed
	public void loadNeighbors(RoomNode roomNode)
	{
		if (roomNode.north != null && !map.roomsActive.Contains(roomNode.north))
		{
			if (loadRoom(roomNode.north))
			{
				// align positions of doorways properly
				GameObject rNorthObj = roomNode.north.obj;
				rNorthObj.transform.position = roomNode.obj.transform.Find("N_transition").position;
				rNorthObj.transform.position -= rNorthObj.transform.Find("S_transition").position - rNorthObj.transform.position;
				// set up doorway triggers
				GameObject trigger = roomNode.obj.transform.Find("N_transition").FindChild("trigger").gameObject;
				trigger.SetActive(true);
				trigger.GetComponent<Doorway>().sideA = roomNode;
				trigger.GetComponent<Doorway>().sideB = roomNode.north;
			}
		}
		if (roomNode.south != null && !map.roomsActive.Contains(roomNode.south))
		{
			if (loadRoom(roomNode.south))
			{
				// align positions of doorways properly
				GameObject rSouthObj = roomNode.south.obj;
				rSouthObj.transform.position = roomNode.obj.transform.Find("S_transition").position;
				rSouthObj.transform.position -= rSouthObj.transform.Find("N_transition").position - rSouthObj.transform.position;
				// set up doorway triggers
				GameObject trigger = roomNode.obj.transform.Find("S_transition").FindChild("trigger").gameObject;
				trigger.SetActive(true);
				trigger.GetComponent<Doorway>().sideA = roomNode;
				trigger.GetComponent<Doorway>().sideB = roomNode.south;
			}
		}
		if (roomNode.east != null && !map.roomsActive.Contains(roomNode.east))
		{
			if (loadRoom(roomNode.east))
			{
				// align positions of doorways properly
				GameObject rEastObj = roomNode.east.obj;
				rEastObj.transform.position = roomNode.obj.transform.Find("E_transition").position;
				rEastObj.transform.position -= rEastObj.transform.Find("W_transition").position - rEastObj.transform.position;
				// set up doorway triggers
				GameObject trigger = roomNode.obj.transform.Find("E_transition").FindChild("trigger").gameObject;
				trigger.SetActive(true);
				trigger.GetComponent<Doorway>().sideA = roomNode;
				trigger.GetComponent<Doorway>().sideB = roomNode.east;
			}
		}
		if (roomNode.west != null && !map.roomsActive.Contains(roomNode.west))
		{
			if (loadRoom(roomNode.west))
			{
				// align positions of doorways properly
				GameObject rWestObj = roomNode.west.obj;
				rWestObj.transform.position = roomNode.obj.transform.Find("W_transition").position;
				rWestObj.transform.position -= rWestObj.transform.Find("E_transition").position - rWestObj.transform.position;
				// set up doorway triggers
				GameObject trigger = roomNode.obj.transform.Find("W_transition").FindChild("trigger").gameObject;
				trigger.SetActive(true);
				trigger.GetComponent<Doorway>().sideA = roomNode;
				trigger.GetComponent<Doorway>().sideB = roomNode.west;
			}
		}
	}

	// Called every time a player walks into a new room. Looks at all currently loaded rooms and unloads the ones that players aren't in or adjacent to
	public void unloadEmptyRooms()
	{
		List<RoomNode> roomsToUnload = new List<RoomNode>();
		foreach (RoomNode loadedRoom in map.roomsActive)
		{
			bool keep = false;
			foreach (RoomNode playerRoom in getRoomsPlayersIn())
			{
				if (playerRoom == loadedRoom)
				{
					keep = true;
				}
				foreach (RoomNode neighbor in playerRoom.neighbors)
				{
					if (neighbor == loadedRoom)
					{
						keep = true;
					}
				}
			}
			if (!keep)
			{
				roomsToUnload.Add(loadedRoom);
			}
		}
		foreach (RoomNode n in roomsToUnload)
		{
			unloadRoom(n);
		}
	}

	private void unloadRoom(RoomNode roomNode)
	{
		roomNode.obj.SetActive (false);
		map.roomsActive.Remove(roomNode);
	}

	// Called whenever a player enters a room. Activates the spawners for that room if a player is entering it for the first time
	public void notifySpawners(RoomNode roomEntered)
	{
		foreach (EnemySpawner es in map.rooms[map.rooms.IndexOf(roomEntered)].enemySpawners)
		{
			if (!es.ableToSpawn)
			{
				es.enableSpawning();
				es.parentRoom = roomEntered.obj.transform;
			}
		}
	}

	// Returns a list of RoomNodes of the rooms that the players are currently located in
	public List<RoomNode> getRoomsPlayersIn()
	{
		List<RoomNode> roomsIn = new List<RoomNode>();
		foreach (GameObject player in GameObject.Find("PlayerManager").GetComponent<PlayerManager>().players)
		{
			RoomNode location = player.GetComponent<PlayerBase>().roomIn;
			if (!roomsIn.Contains(location))
			{
				roomsIn.Add(location);
			}
		}
		return roomsIn;
	}

	// Called whenever a player enters a room. If this room is closer to the end of the dungeon, it assigns new respawn points for the players
	public void updateRespawnPoints(RoomNode room)
	{
		int roomIdx = map.rooms.IndexOf(room);
		if (roomIdx > playerSpawnRoom)
		{
			playerSpawnRoom = roomIdx;
			pMan.assignNewSpawnPoints(room.playerRespawns.ToArray());
		}
	}
}
