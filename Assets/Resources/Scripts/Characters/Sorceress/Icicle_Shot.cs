using UnityEngine;
using System.Collections;
using Exploder;

public class Icicle_Shot : MonoBehaviour 
{

	public float velocity = 45.0f;
	public float rotVel = 45.0f;
	private float iceDamage = 35.0f;

	void Start(){
		Destroy (gameObject, 5.0f);
	}

	void Update () {
		transform.position += (transform.up * velocity * Time.deltaTime);
	}


	void OnTriggerEnter(Collider c){
		if (c.gameObject.CompareTag ("Enemy")) {
			c.gameObject.SendMessage ("takeDamage", iceDamage);
			c.gameObject.SendMessage ("slow");
			Destroy (gameObject);
		} 
		if (c.GetComponent<Explodable>() != null)
		{
			c.SendMessage("Boom");
		}
	}
}
