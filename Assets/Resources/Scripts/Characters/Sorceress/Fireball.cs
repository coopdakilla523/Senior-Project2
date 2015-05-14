using UnityEngine;
using System.Collections;
using Exploder;

public class Fireball : MonoBehaviour {

	public float speed = 15.0f;
	private float fireballDamage = 50.0f;

	void Start(){
		Destroy (gameObject, 5.0f);
	}

	void Update () 
	{
		transform.position = transform.position + (transform.forward * speed * Time.deltaTime);
	}

	void OnTriggerEnter(Collider c){
		if (c.gameObject.CompareTag ("Enemy")) {
			c.gameObject.SendMessage ("takeDamage", fireballDamage);
			Destroy (gameObject);
		}
		if (c.GetComponent<Explodable>() != null)
		{
			c.SendMessage("Boom");
		}

	}
}
