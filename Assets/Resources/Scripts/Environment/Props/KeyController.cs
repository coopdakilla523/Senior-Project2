using UnityEngine;
using System.Collections;

public class KeyController : MonoBehaviour {
		
	void Update () {
		transform.Rotate (new Vector3 (0, 0, 30) * Time.deltaTime);
	}

	void OnTriggerEnter(Collider player)
	{
		if (player.gameObject.CompareTag ("Player")) 
		{
			player.gameObject.SendMessage ("addKey");
			Destroy(gameObject);
		}
	}
}
