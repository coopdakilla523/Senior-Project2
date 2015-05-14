using UnityEngine;
using System.Collections;
using Exploder;

public class Woodsman : PlayerBase
{
	// objects init
	private Transform shootPosition;
	private GameObject hawk;
	private Transform hawkPos;
	private HawkAI2 hawkScripts;	
	public Animator anim;
	private LineRenderer lr;
	private GameObject bomb;
	public ObjectPool pool;

	// bool inits
	private bool canFire = true;
	private bool canSpecial = true;
	private bool attacking = false;
	private bool canMove = true;
	private bool bombActive = false;

	// init variables
	private float timeBNAttacks = 5.0f;
	private float basicTimer = 0.5f;
	private float specialTimer = 3.0f;
	private float firstButtonPressTime = 0.0f;
	public float zeroMoveTimer = 0.0f;
	private float canMoveTimer = 0.0f;
	private int lineSegments = 2;
	private float lineWidth = 0.09f;
	private float lineLength = 35.0f;
	private Vector3 lineEndPoint = Vector3.zero;
	public int hitCount = 0;

	public void Awake()
	{
		classType = playerClass.WOODSMAN;

		anim = GetComponent<Animator>();
		
		// instantiate the hawk at the hawkspawn position
		hawkPos = transform.Find("hawkSpawn");
		hawk = Instantiate(Resources.Load("Prefabs/Character/WoodsMan/Hawk"),hawkPos.position,Quaternion.identity) as GameObject;
		
		// acquire the position from where to shoot arrows
		shootPosition = transform.Find("shootPos");
		
		// set the moveTimer to 0 at the beginning
		canMoveTimer = 0.0f;
		
		// Get the hawk script to be able to set modes
		hawkScripts = hawk.GetComponent<HawkAI2>();
	}

	protected override void Update()
	{
		// call update of parent class
		base.Update();

		timeBNAttacks -= Time.deltaTime;
		if(timeBNAttacks <= 0.0f)
		{
			timeBNAttacks = 5.0f;
			hawkScripts.enemiesToAttack.Clear();
		}

		if(hitCount >= 5)
		{
			Debug.Log ("Added Mana");
			addMana(1.0f);
			hitCount = 0;
		}

		hawk.transform.position = new Vector3 (hawk.transform.position.x, hawkPos.position.y, hawk.transform.position.z);
		if (!canFire) 
		{
			basicTimer -= Time.deltaTime;
			if(basicTimer <= 0.0f)
			{
				canFire = true;
				basicTimer = 0.5f / attackSpeed;
			}
		}
		
		if (!canSpecial) 
		{
			specialTimer -= Time.deltaTime;
			if(specialTimer <= 0.0f)
			{
				canSpecial = true;
				specialTimer = 3.0f;
			}
			
		}


		if(!canMove)
		{
			canMoveTimer += Time.deltaTime;
			if(canMoveTimer >= 0.04f)
			{
				moveMulti = 0.3f;
			}
		}

		//Debug.Log (hawkCost);

	}

	public override void basicAttack(string dir)
	{
		//Debug.Log ("warrior basic attack");
		if (dir == "down" && canFire) 
		{
			firstButtonPressTime = Time.time;
			timeBNAttacks = 5.0f;
			canMove = false;
		}
		if (dir == "up")
		{
			float temp = Time.time - firstButtonPressTime;
			firstButtonPressTime = Time.time;
			canMove = true;
			moveMulti = 1.0f;
			canMoveTimer = 0.0f;
			if(temp > 0.7f / attackSpeed && canSpecial)
			{
				specialAttackWoods(temp);
			}
			else if(canFire)
			{
				anim.SetTrigger("Attack");
				GameObject arrow = pool.New();
				if(arrow != null)
				{
					arrow.GetComponent<BasicArrow>().basic = true;
				}
				canFire = false;
			}
		}
	}
	
	public void specialAttackWoods(float time)
	{
		anim.SetTrigger("Attack");

		GameObject[] arrows = new GameObject[3];
		for(int i = 0; i < arrows.Length; i++)
		{
			arrows[i] = pool.New();
		}
		if(arrows[0] != null)
		{
			arrows[0].GetComponent<BasicArrow>().basic = false;
		}
		Vector3 angle = new Vector3 (0.0f, 12.0f, 0.0f);
		if(arrows[1] != null)
		{
			arrows[1].GetComponent<BasicArrow>().basic = false;
			arrows[1].transform.Rotate(angle);
		}
		if(arrows[2] != null)
		{
			arrows[2].GetComponent<BasicArrow>().basic = false;
			arrows[2].transform.Rotate(angle * -1);

		}
		canSpecial = false;
	}
	
	public override void classAbility(string dir)
	{
		if (dir == "down") 
		{
			GameObject go = GameObject.Find ("Prop_SM_Table_A");
			if(go)
			{
				go.GetComponent<Explodable>().SendMessage("Boom");
				Debug.LogWarning("BOOOOOOOOOM!!!!!!!!!!");
			}
			else
			{
				Debug.LogWarning("No BOOOOOOOOOM!!!!!!!!!!");
			}

			if(!bombActive && mana >= 1.0f)
			{
				mana -= 1.0f;
				Debug.Log (mana);
				//bomb = Instantiate(Resources.Load ("Prefabs/Character/WoodsMan/bomb"),new Vector3(transform.position.x,0.0f,transform.position.z),Quaternion.identity) as GameObject;
				bombActive = true;
			}
			else
			{
				//bombBehavior scr = bomb.GetComponent<bombBehavior>();
				//scr.explode = true;
				bombActive = false;
			}
//			if (hawkScripts.mode != 2 && hawkScripts.mode != 3  && mana > hawkCost) 
//			{
//				anim.SetTrigger("Hawk");
//				hawkScripts.mode = 2;
//			}
		}
	}
}
