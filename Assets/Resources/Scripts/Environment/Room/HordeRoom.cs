using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HordeRoom : MonoBehaviour 
{
	private List<EnemySpawner> roomSpawners;
	private DoorController roomDoor;
	private bool doneSpawning = false;
	private bool enemiesAlive = true;
	private bool doorOpen = false;

	void Start()
	{
		// Get references to all of the enemy spawners in the room
		roomSpawners = new List<EnemySpawner>();
		GameObject[] es = GameObject.FindGameObjectsWithTag("EnemySpawn");
		foreach (GameObject go in es)
		{
			if (go.transform.root == transform)
			{
				roomSpawners.Add(go.GetComponent<EnemySpawner>());
			}
		}

		// Get a reference to the locked door in the room
		roomDoor = transform.Find("Door").GetComponentInChildren<DoorController>();
	}

	void Update()
	{
		if (!doorOpen)
		{
			// Check to see if the enemies are done spawning
			if (!doneSpawning)
			{
				doneSpawning = true;
				foreach (EnemySpawner es in roomSpawners)
				{
					if (es.enemiesRemaining)
					{
						doneSpawning = false;
						break;
					}
				}
			}
			// Once enemies are done spawning, check to see if there are any live enemies in the room
			else if (enemiesAlive)
			{
				EnemyBase enemies = transform.GetComponentInChildren<EnemyBase>();
				if (enemies == null)
				{
					enemiesAlive = false;
				}
			}
			// When all spawners are spent and all enemies are dead, open the door
			if (doneSpawning && !enemiesAlive)
			{
				GameObject.Find("PlayerManager").GetComponent<PlayerManager>().haveKey = true;
				if (roomDoor != null)
				{
					roomDoor.ActivateTrigger(true);
				}
				doorOpen = true;
			}
		}
	}

	public void startSpawning()
	{
		foreach (EnemySpawner es in roomSpawners)
		{
			es.enableSpawning();
		}
	}
}
