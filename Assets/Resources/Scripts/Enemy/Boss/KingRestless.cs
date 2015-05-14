using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class KingRestless : EnemyBase
{
	private Animator myAnimator;
	private GameObject closestPlayer;
	public GameObject healthBarObject;

	// basic attack
	private float basicAttackDamage = 20.0f;
	private float basicAttackRange = 1.5f;

	// home run 
	private float homerunAttackDamage = 80.0f;
	private float homeRunAttackRange = 1.0f;

	// whirlwind
	private float whirlwindSpeedMod = 0.8f;
	private float whirlwindAttackRange = 2.5f;
	private float whirlwindDamageRange = 1.5f;
	private float whirlwindDamage = 15.0f;
	private float whirlwindForceRange = 8.0f;
	private float whirlwindForceMagnitude = 3.0f;
	private float whirlwindInterval = 0.2f;
	private bool spinning = false;

	// shockwave
	public GameObject shockwavePrefab;
	private float shockwaveAttackRange = 3.0f;
	private float shockwaveSpawnDistance = 0.25f;

	// room collapse
	public GameObject ceilingBoulder;
	private bool roomCollapsing = false;
	private float boulderError = 4.0f;
	private float boulderFallIntervalLow = 0.4f;
	private float boulderFallIntervalHigh = 0.8f;
	private float boulderFallHeight = 14.0f;

	//firestorm
	public GameObject firestormPrefab;
	private GameObject firestormInstance;
	private bool firestorming = false;

	// general
	private bool attackInProgress = false;
	private Transform roomCenter;
	private int lastAttack = -1;
	private int currentAttack = -1;			// the index of the attack in progress
											// -1 = no attack in progress
											// 0 = basic attack
											// 1 = homerun
											// 2 = shockwave
											// 3 = whirlwind


	protected override void Start()
	{
		base.Start();
		myAnimator = GetComponent<Animator>();
		roomCenter = GameObject.Find("Boss Room Center").transform;
		healthBarObject.transform.FindChild("BossNameText").GetComponent<Text>().text = "King Restless";
		healthBarObject.SetActive(true);
	}

	void OnDestroy()
	{
		if (healthBarObject)
		{
			healthBarObject.SetActive(false);
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (!attackInProgress)
		{
			// Check for phase attacks first
			if (health <= maxHealth * 0.3f && !firestorming)
			{
				// Move to the center of the room
				myAnimator.SetTrigger("walk");
				moveToPosition(roomCenter.position, Time.deltaTime);
				rotateTowardsPoint(roomCenter.position, Time.deltaTime);

				//do attack
				float dist = Vector3.Distance(transform.position, roomCenter.position);
				if (dist < 0.4f)
				{
					attackInProgress = true;
					firestorming = true;
					myAnimator.SetTrigger("firestorm");

					Vector3 firestormCenter = roomCenter.position;
					firestormInstance = Instantiate(firestormPrefab, firestormCenter, Quaternion.identity) as GameObject;
					firestormInstance.transform.parent = roomCenter.root;
				}
			}
			else if (health <= maxHealth * 0.6f && !roomCollapsing)
			{
				// Move to the center of the room
				myAnimator.SetTrigger("walk");
				moveToPosition(roomCenter.position, Time.deltaTime);
				rotateTowardsPoint(roomCenter.position, Time.deltaTime);

				// HULK SMASH
				float dist = Vector3.Distance(transform.position, roomCenter.position);
				if (dist < 0.4f)
				{
					attackInProgress = true;
					roomCollapsing = true;
					myAnimator.SetTrigger("roomCollapse");
				}
			}

			// If no phase attacks, set up another attack
			else
			{
				// If no attack currently selected, pick one at random
				if (currentAttack == -1)
				{
					float randomTemp = Random.Range(0.0f, 100.0f);

					if (randomTemp < 55.0f)
					{
						currentAttack = 0; // basic attack
					}
					else if (randomTemp < 70.0f)
					{
						currentAttack = 1; // homerun
					}
					else if (randomTemp < 85.0f)
					{
						currentAttack = 2; // shockwave
					}
					else
					{
						currentAttack = 3; // whirlwind
					}
					// if we picked the same attack as last time, just do a basic attack
					if (currentAttack == lastAttack)
					{
						currentAttack = 0;
					}
					lastAttack = currentAttack;
				}

				// Once an attack is set up, move to the appropriate location and begin attack when close enough
				if (currentAttack == 0)
				{
					if (find(basicAttackRange))
					{
						attackInProgress = true;
						myAnimator.SetTrigger("basicAttack");
					}
				}
				else if (currentAttack == 1)
				{
					if (find(homeRunAttackRange))
					{
						attackInProgress = true;
						myAnimator.SetTrigger("homerun");
					}
				}
				else if (currentAttack == 2)
				{
					if (find(shockwaveAttackRange))
					{
						attackInProgress = true;
						myAnimator.SetTrigger("shockwave");
					}
				}
				else if (currentAttack == 3)
				{
					if (find(whirlwindAttackRange))
					{
						attackInProgress = true;
						myAnimator.SetTrigger("whirlwind");
					}
				}
			}
		}
	}

	void OnGUI()
	{

	}

	private bool find(float attackRange)
	{
		closestPlayer = findClosestPlayer();
		moveTowardsPlayer(closestPlayer, Time.deltaTime);
		rotateTowardsPlayer(closestPlayer, Time.deltaTime);
		if (Vector3.Magnitude(closestPlayer.transform.position - transform.position) < attackRange)
		{
			return true;
		}
		myAnimator.SetTrigger("walk");
		return false;
	}

	public override void kill()
	{
		StopAllCoroutines();
		Destroy(firestormInstance);
		base.kill();
	}

	public void startShockwave()
	{
		GameObject shockwave = Instantiate(shockwavePrefab, transform.position + shockwaveSpawnDistance * transform.forward, Quaternion.LookRotation(transform.forward)) as GameObject;
		shockwave.transform.parent = roomCenter.root;
	}

	public void startWhirlwind()
	{
		spinning = true;
		StartCoroutine(whirlwindAttack());
		StartCoroutine(whirlwindPlayerSeek());
	}

	public void endWhirlwind()
	{
		spinning = false;
	}

	private IEnumerator whirlwindAttack()
	{
		LayerMask playerMask = LayerMask.GetMask(new string[]{"Player"});

		while (spinning)
		{
			// Draw all players in within a large range
			Collider[] hit = Physics.OverlapSphere(transform.position, whirlwindForceRange, playerMask);
			foreach (Collider c in hit)
			{
				Vector3 fromPlayer = (transform.position - c.transform.position).normalized;
				fromPlayer *= whirlwindForceMagnitude;
				c.GetComponent<CharacterBase>().addForce(fromPlayer);
			}
			// Damage all players in a small sphere
			hit = Physics.OverlapSphere(transform.position, whirlwindDamageRange, playerMask);
			foreach (Collider c in hit)
			{
				c.SendMessage("takeDamage", whirlwindDamage);
			}
			yield return new WaitForSeconds(whirlwindInterval);
		}
	}

	private IEnumerator whirlwindPlayerSeek()
	{
		moveMulti *= whirlwindSpeedMod;
		while (spinning)
		{
			GameObject target = findClosestPlayer();
			moveTowardsPlayer(target, Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		moveMulti /= whirlwindSpeedMod;
		yield return null;
	}

	public void basicAttack()
	{
		Collider[] hit = Physics.OverlapSphere(transform.position + transform.forward, 1.0f, LayerMask.GetMask("Player"));
		foreach (Collider c in hit)
		{
			c.SendMessage("takeDamage", basicAttackDamage);
		}
	}

	public void startRoomCollapse()
	{
		StartCoroutine(roomCollapseAttack());
	}

	private IEnumerator roomCollapseAttack()
	{
		while (roomCollapsing)
		{
			List<GameObject> playerList = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().players;
			int playerIdx = Random.Range(0, playerList.Count);
			Vector3 playerPos = playerList[playerIdx].transform.position;
			Vector3 boulderPos = new Vector3(playerPos.x + Random.Range(-boulderError, boulderError), playerPos.y + boulderFallHeight, playerPos.z + Random.Range(-boulderError, boulderError));
			GameObject boulder = Instantiate(ceilingBoulder, boulderPos, Quaternion.identity) as GameObject;
			boulder.transform.parent = roomCenter.root; // make sure the boulder is spawned as a child of the current room
			yield return new WaitForSeconds(Random.Range(boulderFallIntervalLow, boulderFallIntervalHigh));
		}
		yield return null;
	}

	public void homerunAttack()
	{
		Collider[] hit = Physics.OverlapSphere(transform.position + transform.forward, 1.0f, LayerMask.GetMask("Player"));
		foreach (Collider c in hit)
		{
			c.SendMessage("takeDamage", homerunAttackDamage);
			c.SendMessage("addForce", (c.transform.position - transform.position).normalized * 35.0f);
		}
	}

	public void notifyAttackEnd()
	{
		currentAttack = -1;
		attackInProgress = false;
	}
}
