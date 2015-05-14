using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {

	public void ActivateTrigger(bool state)
	{
		if(this.CompareTag("door"))
		{
			PlayerManager pm = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
			if(pm.haveKey)
			{
				pm.haveKey = false;
				Destroy(this.gameObject);
			}
		}
		else
		{
			Destroy(this.gameObject);
		}
	}
}
