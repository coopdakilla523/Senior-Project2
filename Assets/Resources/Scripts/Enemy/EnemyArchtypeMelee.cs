using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyArchtypeMelee : EnemyBase 
{
	public Transform target;
	private Transform mTransform;
	
	public float chaseRange = 20.0f;
	public float pDistance;
	public GameObject player;

	// Behavior / Rates
	private bool chasing = false;

	void Awake() 
	{
		mTransform = transform;
	}

	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		player = findClosestPlayerInRange (chaseRange);
		if (player != null && !attacking)
		{
			target = player.transform;
			pDistance = (target.position - mTransform.position).magnitude;
		
			if (chasing) 
			{
				if(pDistance > giveUpThreshold)
				{
					chasing = false;
					GetComponent<Animator>().SetBool("walking", false);
				}

				if(!attacking && pDistance <= attackDistance)
				{
					GetComponent<Animator>().SetTrigger("attack");
					attacking = true;
				}
				else
				{
					cc.Move(mTransform.forward * moveSpeed * Time.deltaTime * moveMulti);
					rotateTowardsPlayer(player, Time.deltaTime);
				}
			}
			else
			{
				if(pDistance < chaseRange)
				{
					chasing = true;
					GetComponent<Animator>().SetBool("walking", true);
				}
			}
		}
	}

	private void Attack()
	{
		Collider[] hit = Physics.OverlapSphere(transform.position + transform.forward, 1.0f, LayerMask.GetMask("Player"));
		foreach (Collider c in hit)
		{
			c.GetComponent<PlayerBase>().takeDamage(attackDamage);
		}
	}

	public void notifyAttackEnd()
	{
		attacking = false;
	}
}