using UnityEngine;
using System.Collections;
using Exploder;

public class woodsSpecialBulletScript : MonoBehaviour {
	

	private GameObject woodsPlayer;
	private GameObject hawk;
	private HawkAI2 hawkScript;

	private float speed = 15.0f;
	public int numPiercing = 0;
	public float heldTime = 0.0f;
	private bool infinitePierce = false;
	public Vector3 playerForward;
	private float dmg = 35.0f;
	private float timer = 7.0f;
	// Use this for initialization
	void Start () 
	{
		GameObject playerManager = GameObject.FindGameObjectWithTag("PlayerManager");
		PlayerManager playerManagerScript = playerManager.GetComponent<PlayerManager> ();
		for (int i=0; i<playerManagerScript.numPlayers; i++)
		{
			if(playerManagerScript.players[i].GetComponent<PlayerBase>().classType == playerClass.WOODSMAN)
			{
				woodsPlayer = playerManagerScript.players[i];
			}
		}
		playerForward = woodsPlayer.transform.forward;
		transform.up = new Vector3(playerForward.x, playerForward.y, playerForward.z);
		if (heldTime > 5.0f) 
		{
			infinitePierce = true;
		} 
		else 
		{
			numPiercing = Mathf.FloorToInt(heldTime);
		}

		hawk = GameObject.FindGameObjectWithTag ("Hawk");
		hawkScript = hawk.GetComponent<HawkAI2> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = transform.position + (playerForward * speed * Time.deltaTime);
		timer -= Time.deltaTime;
		if(timer <= 0.0f)
		{
			Destroy (gameObject);
		}
	}
	
	void OnTriggerEnter(Collider c)
	{
		Debug.Log ("hi");
		if (c.collider.GetComponent<Explodable>() != null)
		{
			c.collider.SendMessage("Boom");
		}
		if(c.gameObject.CompareTag("wall"))
		{
			Destroy (gameObject);
		}
		if(c.gameObject.CompareTag("Enemy"))
		{
			Woodsman tempScr = woodsPlayer.GetComponent<Woodsman>();
			EnemyBase scr = c.gameObject.GetComponent<EnemyBase>();
			scr.takeDamage(dmg);
			scr.damageTaken += dmg;
			tempScr.hitCount += 1;

			if(hawkScript.enemiesToAttack.Contains(c.gameObject) == false)
			{
				hawkScript.enemiesToAttack.Add (c.gameObject);
			}

			if(infinitePierce == false)
			{
				if(numPiercing >0)
				{
					numPiercing = numPiercing -1;

				}
				else
				{

					Destroy(gameObject);
				}
			}
		}
	}
}

