using UnityEngine;
using System.Collections;
using Exploder;

public class Warrior : PlayerBase
{
	private bool blockProjectiles = false;
	private int count = 0;
	private int attackType = 1;
	private float attackStarted = Time.time - 10.0f;
	private float comboStarted = Time.time - 10.0f;
	private bool attacking = false;
	private bool canAttack = true;

	private float specialChargeTime = 0.8f;

	private float normalAttackDamage = 25.0f;
	private float comboAttackDamage = 50.0f;
	private float comboAttackForce = 15.0f;
	private float whirlwindMana = 25.0f;
	private float specialAttackDuration = 2.0f;
	private float specialAttackForce = 20.0f;

	private float whirlwindDamageDelay = 0.2f;

	public void Awake()
	{
		classType = playerClass.WARRIOR;
	}

	public override void basicAttack(string dir)
	{
		if (dir == "down")
		{
			//Check enemy facing
			attackStarted = Time.time;
		}
		float currentTime = Time.time;
		float timeSinceAttack = currentTime - attackStarted;
		float timeSinceCombo = currentTime - comboStarted;
		if (dir == "up" && canAttack) {
			//When the attack key is released, check to see how long it was
			//held to determine what attack to do.
			if (timeSinceAttack > specialChargeTime / attackSpeed && checkForMana(whirlwindMana)){
				// special attack
				useMana(whirlwindMana);
				GetComponent<Animator>().SetTrigger("Whirlwind");
			}
			else {
				// continue the combo
				canAttack = false;
				GetComponent<Animator>().SetTrigger("Attack");
			}
		}
	}

	public override void specialAttack()
	{
		StartCoroutine(Whirlwind());
	}

	private IEnumerator Whirlwind()
	{
		while (!canAttack)
		{
			Collider[] hit = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask("Enemy"));
			foreach (Collider c in hit)
			{
				if (c.tag == "Enemy")
				{
					Vector3 forceDir = (c.transform.position - transform.position);
					forceDir = new Vector3(forceDir.x, 0.0f, forceDir.z).normalized;
					c.GetComponent<EnemyBase>().addForce(forceDir * specialAttackForce);
				}
				if (c.GetComponent<Explodable>() != null)
				{
					c.SendMessage("Boom");
				}
			}
			yield return new WaitForSeconds(whirlwindDamageDelay);
		}
	}

	public override void classAbility(string dir)
	{
		if (dir == "down" && canAttack)
		{
			GetComponent<Animator>().SetTrigger("Block");
			moveMulti = 0.5f;
			blockProjectiles = true;
		} 
		else if (dir == "up") 
		{
			GetComponent<Animator>().SetTrigger("Idle");
			moveMulti = 1.0f;
			blockProjectiles = false;
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
		Collider[] hit = Physics.OverlapSphere(transform.position + transform.forward, 1.0f, LayerMask.GetMask("Enemy"));
		foreach (Collider c in hit)
		{
			if (c.tag == "Enemy")
			{
				addMana(5.0f);
				c.GetComponent<EnemyBase>().takeDamage(normalAttackDamage * attackMultiplier);
			}
			if (c.GetComponent<Explodable>() != null)
			{
				c.SendMessage("Boom");
			}
		}
	}

	// Called by an animation event at the start of an end of Attack3 animation
	public void triggerComboAttack()
	{
		Collider[] hit = Physics.OverlapSphere(transform.position + transform.forward * 1.5f, 1.25f, LayerMask.GetMask("Enemy"));
		foreach (Collider c in hit)
		{
			if (c.tag == "Enemy")
			{
				addMana(5.0f);
				c.GetComponent<EnemyBase>().takeDamage(comboAttackDamage * attackMultiplier);
				Vector3 forceDir = (c.transform.position - transform.position);
				forceDir = new Vector3(forceDir.x, 0.0f, forceDir.z).normalized;
				c.GetComponent<EnemyBase>().addForce(forceDir * comboAttackForce);
			}
			if (c.GetComponent<Explodable>() != null)
			{
				c.SendMessage("Boom");
			}
		}
	}

	public void triggerWhirlwind()
	{
		canAttack = false;
		specialAttack();
	}
		
}