using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Rewired;

public class PlayerManager : MonoBehaviour 
{
	// init variables
	public int numPlayers;
	private GameObject[] spawns;
	public List<GameObject> players;
	public List<playerClass> selectedClasses;

	public bool haveKey = false;

	public Woodsman woods;
	public Sorcerer sorc;
	public Warrior war;
	public Rogue rogue;

	public Vector3 playersCenter;

	void Start() 
	{
		// instantiate players list
		players = new List<GameObject>();

		// instantiate selectedClasses and populate it
		selectedClasses = new List<playerClass>();
		selectedClasses.Add(playerClass.WOODSMAN);
		selectedClasses.Add(playerClass.ROGUE);
		selectedClasses.Add(playerClass.SORCERER);
		selectedClasses.Add(playerClass.WARRIOR);

		// set players to 4 at start
		GameObject newStatBar = GameObject.Find("newStatBar");
		for (int i=0; i<numPlayers; i++) 
		{
			GameObject player = null;
			PlayerBase pBase;
			Texture manaTex = null;
			Texture profTex = null;

			switch(selectedClasses[i])
			{
			case playerClass.WOODSMAN:
				player = Instantiate(Resources.Load("Prefabs/Character/WoodsMan/Woodsman"), spawns[i].transform.position, Quaternion.identity) as GameObject;
				manaTex = Resources.Load("Textures/GUI/gui_energy_woods") as Texture;
				profTex = Resources.Load("Textures/GUI/archer_profile") as Texture;
				break;
			case playerClass.SORCERER:
				player = Instantiate(Resources.Load("Prefabs/Character/Sorceress/Sorceress"), spawns[i].transform.position, Quaternion.identity) as GameObject;
				manaTex = Resources.Load("Textures/GUI/gui_energy_bar") as Texture;
				profTex = Resources.Load("Textures/GUI/sorc_profile") as Texture;
				break;
			case playerClass.ROGUE:
				player = Instantiate(Resources.Load("Prefabs/Character/Rogue/Rogue"), spawns[i].transform.position, Quaternion.identity) as GameObject;
				manaTex = Resources.Load("Textures/GUI/gui_energy_rogue") as Texture;
				profTex = Resources.Load("Textures/GUI/rogue_profile") as Texture;
				break;
			case playerClass.WARRIOR:
				player = Instantiate(Resources.Load("Prefabs/Character/Warrior/Warrior"), spawns[i].transform.position, Quaternion.identity) as GameObject;
				manaTex = Resources.Load("Textures/GUI/gui_energy_war") as Texture;
				profTex = Resources.Load("Textures/GUI/warrior_profile") as Texture;
				break;
			}

			players.Add(player);

			player.tag = "Player";
			pBase = player.GetComponent<PlayerBase>();
			pBase.playerNum = i;
			player.AddComponent<rewiredControl>();

			GameObject stats = GameObject.Find("Player" + (i+1));
			stats.name = pBase.classType.ToString();

			CharacterStats cStat = stats.GetComponent<CharacterStats>();
			cStat.player = pBase;

			cStat.healthBar.enabled = true;
			cStat.manaBar.texture = manaTex;
			cStat.manaBar.enabled = true;
			cStat.potion.enabled = true;
			RawImage prof = stats.transform.Find("Emblem/Character").GetComponent<RawImage>();
			prof.texture = profTex;
			prof.enabled = true;
			cStat.scoreText.enabled = true;
		}
	}

	public void Update() 
	{
		updateCenterLocation();
	}

	public Vector3 getRespawnPoint()
	{
		int randSpawn = Random.Range (0,4);
		return spawns[randSpawn].transform.position;
	}

	public void assignNewSpawnPoints(GameObject[] newSpawns)
	{
		spawns = newSpawns;
	}

	public void respawnAllPlayers()
	{
		for (int i = 0; i < players.Count; i++)
		{
			players[i].transform.position = spawns[i].transform.position;
			players[i].GetComponent<PlayerBase>().controllable = true;
		}
	}

	public void updateCenterLocation()
	{
		Vector3 center = Vector3.zero;
		for (int i = 0; i <  players.Count; i++) 
		{
			center = center + players[i].transform.position;
		}
		center = center / players.Count;

		playersCenter = center;
		GameObject.Find ("Center").transform.position = center;
	}
}
