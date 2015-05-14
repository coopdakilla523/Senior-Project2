using UnityEngine;
using System.Collections;
using Exploder;

public class Rogue : PlayerBase
{
	private float attackStarted = Time.time - 10.0f;
	private bool dash = false;
	public float dashDur = 0.3f;
	public float dashSpeed = 30.0f;
	public float dashMana = 30.0f;
	public float stealthMana = 8.0f;
	private float elapsed = 0.0f;
	private bool canAttack = true;
	private float normalAttackDamage = 10.0f;

	public void Awake()
	{
		classType = playerClass.ROGUE;
	}

	public override void basicAttack(string dir)
	{
		Debug.Log ("attacking");
		if(dir == "down")
		{
			//Check enemy facing
			Debug.Log ("rogue attack");
			attackStarted = Time.time;
		}
		float currentTime = Time.time;
		float timeSinceAttack = currentTime - attackStarted;
		//When the attack key is released, check to see how long it was
		//held to determine what attack to do.
		if (dir == "up" && canAttack)
		{
			if(timeSinceAttack < 1.0f / attackSpeed || !checkForMana(dashMana))
			{
				Debug.Log ("rogue basic attack");
				//Basic Attack
				//animator.Play("RogueBasicAttack");
				canAttack = false;
				visibility  = 1.0f;
				GetComponent<Animator>().SetTrigger("Attack");
			}
			else
			{
				Debug.Log ("rogue special attack");
				//Dash Attack
				dash = true;
				controllable = false;
				specialAttack();
				GetComponent<Animator>().SetTrigger("Dash");
			}
		}
	}

	public override void specialAttack()
	{
		useMana(dashMana);
		StartCoroutine(Dash());
	}



	public IEnumerator Dash2()
	{
		yield return new WaitForSeconds (dashDur);
		dash = false;
		controllable = true;
		GetComponent<Animator>().SetTrigger("Idle");
	}

	public void FixedUpdate2()
	{
		if(dash)
		{
			Debug.Log ("dashing");
			elapsed += Time.deltaTime;
			Vector3 moveVec = transform.forward * dashSpeed * Time.deltaTime;
			moveVec = new Vector3(moveVec.x, verticalVelocity, moveVec.z);
			//cc.Move(moveVec);
			
			
			PlayerBase character = this.GetComponent<PlayerBase>();
			PlayerManager plyrMgr = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
			int maxDist = 20; //hardcoded value - based on maxDist in the rewiredControl script
			moveVec.y = 0.0f;
			
			
			Vector3 newLocation = moveVec * character.moveMulti;
			float newDistFromCenter = Vector3.Distance(newLocation + character.transform.position, plyrMgr.playersCenter);
			//Debug.Log("New Dist: " + newDistFromCenter + " Cur Dist: " + Vector3.Distance(character.transform.position, plyrMgr.playersCenter) + "Max Dist: " + maxDist);
			// If the player is moving too far away from the center, they are stopped. If they're already
			// too far away, they are only allowed to move closer to the center.
			if (newDistFromCenter <= maxDist || newDistFromCenter < Vector3.Distance(character.transform.position, plyrMgr.playersCenter)) 
			{
				cc.Move(newLocation);
				//character.addForce(moveVector);// * moveSpeed * Time.deltaTime * character.moveMulti);
			}
		}
	}
	
	public IEnumerator Dash()
	{
		while(dash && elapsed < dashDur)
		{
			Debug.Log ("dashing");
			elapsed += Time.deltaTime;
			Vector3 moveVec = transform.forward * dashSpeed * Time.deltaTime;
			moveVec = new Vector3(moveVec.x, verticalVelocity, moveVec.z);
			//cc.Move(moveVec);


			PlayerBase character = this.GetComponent<PlayerBase>();
			PlayerManager plyrMgr = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
			int maxDist = 20; //hardcoded value - based on maxDist in the rewiredControl script
			moveVec.y = 0.0f;
			

			Vector3 newLocation = moveVec * character.moveMulti;
			float newDistFromCenter = Vector3.Distance(newLocation + character.transform.position, plyrMgr.playersCenter);
			// If the player is moving too far away from the center, they are stopped. If they're already
			// too far away, they are only allowed to move closer to the center.
			if (newDistFromCenter <= maxDist || newDistFromCenter < Vector3.Distance(character.transform.position, plyrMgr.playersCenter)) 
			{
				cc.Move(newLocation);
			}

			yield return new WaitForFixedUpdate();
		}
		dash = false;
		controllable = true;
		elapsed = 0.0f;
		GetComponent<Animator>().SetTrigger("Idle");
	}
	
	public override void classAbility(string dir)
	{
		Debug.Log ("rogue class ability");
		//Make the rogue invisible
		if(dir == "down")
		{
			if(visibility == 1.0f)
			{
				visibility = 0.0f;
				GetComponent<Animator>().SetTrigger("Sneak");
			}
			else if (visibility == 0.0f)
			{
				visibility = 1.0f;
				canAttack = true;
				GetComponent<Animator>().SetTrigger("Idle");
			}
		}
	}

	// Called by an animation event at the end of each attack animation
	public void notifyAttackEnd()
	{
		canAttack = true;
	}
	
	// Called by an animation event at the start of Attack1 and 2 animation
	public void triggerNormalAttack()
	{
		Collider[] hit = Physics.OverlapSphere(transform.position + transform.forward, 0.5f);
		foreach (Collider c in hit)
		{
			if(c.tag == "Enemy")
			{
				Vector3 vec = (transform.position - c.transform.position).normalized;
				float angle = Vector3.Angle(c.transform.forward, vec);
				Debug.Log(angle);
				int bonus = 1;
				if(angle > 90)
				{
					bonus = 2;
				}
				//restore more on back attack
				addMana(4.0f * bonus);
				c.GetComponent<EnemyBase>().takeDamage(normalAttackDamage * bonus*attackMultiplier);
			}
			if (c.GetComponent<Explodable>() != null)
			{
				c.SendMessage("Boom");
			}
		}
	}

	protected override void Update()
	{
		base.Update();

		//Increase the rogue's energy if it is not full and he is visible
		if(visibility == 1.0f && mana < 100.0f)
		{
			manaRegen(8.0f);
		}
		//Deplete the rogue's energy if he is invisible
		else if(visibility == 0.0f)
		{
			if(checkForMana(stealthMana * Time.deltaTime))
			{
				useMana(stealthMana * Time.deltaTime);
			}
			else
			{
				//If the rogue runs out of energy, he becomes visible
				visibility = 1.0f;
				canAttack = true;
				GetComponent<Animator>().SetTrigger("Idle");
			}
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}
	
	protected void OnTriggerEnter(Collider collider)
	{
		GameObject go = collider.gameObject;
		if(go.tag == "Enemy")
		{
			if(dash)
			{
				//if the player cannot go behind the enemy, attempt to go to the right then left
				Debug.Log("hurt enemy");
				dash = false;
				controllable = true;
				elapsed = 0.0f;

				//center the enemy's collider
				Vector3 colCenter = go.GetComponent<Transform>().position;
				colCenter.y = this.transform.position.y;

				//behind facing
				Vector3 colBehind = go.transform.forward;
				colBehind.y = this.transform.forward.y;
				//right facing
				Vector3 colRight = go.transform.right;
				colRight.y = this.transform.forward.y;
				//left facing
				Vector3 colLeft = -1 * colRight;
				colLeft.y = this.transform.forward.y;

				//scales the radius of the enemy's bounding volume
				Vector3 sc = go.transform.localScale;
				float largest = Mathf.Max(sc.x, sc.y, sc.z);
				float colRad = (go.GetComponent<CharacterController>().radius + 0.22f) * largest;
				//scales the radius of the player's bounding volume
				sc = this.transform.localScale;
				largest = Mathf.Max(sc.x, sc.y, sc.z);
				float plRad = this.GetComponent<CharacterController>().radius * largest;
				colRad += plRad;

				//coordinates to move behind the enemy
				Vector3 moveBehind = -1 * colRad * colBehind;
				moveBehind.y = this.transform.position.y;
				//coordinates to move to the left side of the enemy
				Vector3 moveRight = -1 * colRad * colRight;
				moveRight.y = this.transform.position.y;
				//coordinates to move to the right side of the enemy
				Vector3 moveLeft = -1 * colRad * colLeft;
				moveLeft.y = this.transform.position.y;

				//list of collisions for the potential positions of the player
				Collider[][] colList = {Physics.OverlapSphere(colCenter + moveBehind, plRad),
										Physics.OverlapSphere(colCenter + moveRight, plRad),
										Physics.OverlapSphere(colCenter + moveLeft, plRad)};

				bool move = true;
				int i = 0;
				for(i = 0; i < colList.Length; i++)
				{
					move = true;
					for(int j = 0; j < colList[i].Length; j++)
					{
						Debug.Log(colList[i][j].name);
						if(colList[i][j].name.Contains("Wall") || colList[i][j].tag == "Enemy")
						{
							move = false;
							Debug.Log("something is in the way");
							break;
						}
					}
					if(move)
					{
						break;
					}
				}

				if(move)
				{
					if(i == 0)
					{
						Debug.Log("behind");
						this.transform.forward = colBehind;
						this.transform.position = colCenter;
						this.transform.Translate(moveBehind, Space.World);
					}
					else if(i == 1)
					{
						Debug.Log("right");
						this.transform.forward = colRight;
						this.transform.position = colCenter;
						this.transform.Translate(moveRight, Space.World);
					}
					else if(i == 2)
					{
						Debug.Log("left");
						this.transform.forward = colLeft;
						this.transform.position = colCenter;
						this.transform.Translate(moveLeft, Space.World);
					}
				}
				GetComponent<Animator>().SetTrigger("Attack");
			}
			else
			{
				Debug.Log("collided with enemy: " + go.name + " but wasn' dashing...");
			}
		}
		else
		{
			Debug.Log("collided with " + go.name);
		}
	}
}