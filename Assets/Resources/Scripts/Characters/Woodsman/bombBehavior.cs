using UnityEngine;
using System.Collections;

public class bombBehavior : MonoBehaviour {

	public bool explode = false; 
	private float dmg = 37.5f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(explode)
		{
			Collider[] hitColliders = Physics.OverlapSphere(transform.position,7.5f);
			for(int i=0;i<hitColliders.Length;i++)
			{
				if(hitColliders[i].CompareTag("Enemy"))
				{
					hitColliders[i].SendMessage("takeDamage", dmg);
				}

			}
			Destroy(gameObject);
		}
	}
}
