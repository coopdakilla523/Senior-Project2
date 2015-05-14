using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum PotionType
{
	NONE,
	HEALTH,
	HASTE,
	ATTACK,
	BIG

}

public class PotionBase : MonoBehaviour 
{	
	public PotionType type;	// set in the inspector to assign potion type

	public void Start()
	{
	}

	void Update()
	{
		transform.Rotate (new Vector3 (0, 30, 0) * Time.deltaTime);
	}

	void OnTriggerEnter(Collider player)
	{
		if (player.gameObject.CompareTag("Player")) 
		{
			player.gameObject.SendMessage("addItem", type);
			Destroy(gameObject);
		}
	}
}
