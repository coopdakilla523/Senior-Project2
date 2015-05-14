using UnityEngine;
using System.Collections;
using Exploder;

public class MeteorController : MonoBehaviour 
{
	private ParticleSystem p;
	public float rotVel = 45.0f;
	private float radius = 3.0f;
	private float damageAmount = 100.0f;

	void OnTriggerEnter(Collider c)
	{
		// Do an overlap sphere to get all enemies this attack will hit
		Collider[] hit = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Enemy"));
		//foreach (Collider e in hit)
		//{
		//	e.SendMessage("takeDamage", damageAmount);
		//}
		foreach (Collider e in hit)
		{
			if (e.tag == "Enemy")
			{
				e.GetComponent<EnemyBase>().takeDamage(damageAmount);
			}
			if (e.GetComponent<Explodable>() != null)
			{
				e.SendMessage("Boom");
			}
		}
		// Set up the destruction of this object
		p = transform.parent.gameObject.GetComponentInChildren<ParticleSystem>();
		p.loop = false;
		Destroy (gameObject);
		Destroy (transform.parent.gameObject, 5.0f);
	}
}
