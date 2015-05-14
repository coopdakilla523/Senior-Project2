using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum playerClass
{
	WARRIOR, 
	SORCERER, 
	ROGUE, 
	WOODSMAN
};

public class PlayerBase : CharacterBase 
{
	public float respawnTimer = 0.0f;
	public float timeToRespawn = 0.5f;

	public int playerNum = -1;
	public int playerId = -1;

	public bool controllable = true;

	public bool canJump = false;
	public float jumpForce = 15.0f;
	public float verticalVelocity = 0.0f;
	public playerClass classType;

	public PotionType item;

	public float mana = 100.0f;
	public float maxMana = 100.0f;

	public int score = 0;

	protected bool special = false;
	protected bool normal = false;

	public float attackSpeed = 1.0f;

	public RoomNode roomIn;

	// controls
/*	public string moveAxisX;
	public string moveAxisZ;
	public KeyCode jumpKey;
	public KeyCode basicAttackKey;
	public KeyCode useItemKey = KeyCode.Space;
	public KeyCode classAbilityKey;
	public KeyCode specialAttackKey;*/

	public PlayerManager manager;
	
	private MapManager mapMan;

	protected override void Start()
	{
		base.Start();
		manager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
		mapMan = GameObject.Find("MapManager").GetComponent<MapManager>();
		jumpForce = 9.0f;
	}

	protected virtual void Update()
	{
		// Handle respawn timer
		if (dead)
		{
			respawnTimer -= Time.deltaTime;
			if (respawnTimer <= 0.0f)
			{
				respawn();
			}
		}
	}

	public override void takeDamage(float amount)
	{
		// Gives the players only an invuln period after being hit
		if (currentDamageCooldown > 0.0f || dead)
		{
			return;
		}

		base.takeDamage(amount);
	}

	public override void kill()
	{
		score -= 25;
		if (score < 0)
		{
			score = 0;
		}
		base.kill();
		respawnTimer = timeToRespawn;
	}

	public override void respawn()
	{
		transform.position = manager.getRespawnPoint();
		transform.rotation = Quaternion.identity;

		health = maxHealth;

		verticalVelocity = 0.0f;

		dead = false;
		canJump = false;

		base.respawn();
	}

	public void enterRoom(RoomNode room)
	{
		roomIn = room;
		mapMan.notifySpawners(room);
		mapMan.loadNeighbors(room);
		mapMan.unloadEmptyRooms();
		mapMan.updateRespawnPoints(room);

		HordeRoom h = room.obj.GetComponent<HordeRoom>();
		if (h != null)
		{
			h.startSpawning();
		}
	}

	public void addItem(PotionType p)
	{
		item = p;

		if (p == PotionType.HEALTH) 
		{
			gameObject.AddComponent<HealthPotion> ();
		} 
		else if (p == PotionType.HASTE) 
		{
			gameObject.AddComponent<HastePotion> ();
		} 
		else if (p == PotionType.ATTACK) 
		{
			gameObject.AddComponent<AttackUpPotion> ();
		}
	}

	public void addScore(GameObject p)
	{
		if (p.tag == "Coin")
		{
			score += 1;
		}
		if (p.tag == "Gold")
		{
			score += 10;
		}
	}

	private void addKey()
	{
		manager.haveKey = true;
	}

	public void itemAbility()
	{
		if (item != PotionType.NONE) 
		{
			SendMessage("usePotion", this);
			item = PotionType.NONE;
		}
	}

	public void useMana(float amt)
	{
		mana -= amt;
		mana = Mathf.Clamp(mana, 0, maxMana);
	}

	public void addMana(float amt)
	{
		mana += amt;
		mana = Mathf.Clamp(mana, 0, maxMana);
	}

	public bool checkForMana(float amt)
	{
		//takes in an amount of mana to check if attack can occur
		if(mana - amt >= 0)
			return true;
		else
			return false;
	}

	public void manaRegen(float perSec)
	{
		//mana regeneration function for any players with mana regenerate.
		mana += perSec * Time.deltaTime;
		mana = Mathf.Clamp(mana, 0, maxMana);
	}

	public virtual void basicAttack(string dir){}
	public virtual void specialAttack(){}
	public virtual void classAbility(string dir){}



}
