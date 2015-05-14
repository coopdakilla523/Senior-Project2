using UnityEngine;
using System.Collections;

public class HastePotion : MonoBehaviour 
{
	private float speedIncrease = 1.5f;
	private float effectDuration = 10.0f;

	public void usePotion(PlayerBase p)
	{
		StartCoroutine(hasteCoroutine(p));	
	}

	private IEnumerator hasteCoroutine(PlayerBase p)
	{
		GetComponent<Animator>().speed = speedIncrease;
		p.attackSpeed = speedIncrease;
		yield return new WaitForSeconds(effectDuration);
		GetComponent<Animator>().speed = 1.0f;
		p.attackSpeed = 1.0f;
		Destroy(this);
		yield return null;
	}
}
