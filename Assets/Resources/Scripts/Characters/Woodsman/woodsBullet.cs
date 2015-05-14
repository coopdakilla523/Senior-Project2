using UnityEngine;
using System.Collections;
using Exploder;

public class woodsBullet : MonoBehaviour {

	
	private float speed = 20.0f;
	public Vector3 playerForward;
	public GameObject woodsPlayer;
	private float timer;
	private GameObject hawk;
	private HawkAI2 hawkScript;
	private float dmg = 25.0f;
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
		//transform.up = playerForward;
		timer = 1.0f;

		hawk = GameObject.FindGameObjectWithTag ("Hawk");
		hawkScript = hawk.GetComponent<HawkAI2> ();

	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = transform.position + (playerForward * speed * Time.deltaTime);
		timer = timer - Time.deltaTime;
		if (timer <= 0.0f) 
		{
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if (c.collider.GetComponent<Explodable>() != null)
		{
			c.collider.SendMessage("Boom");
		}
		if (c.gameObject.CompareTag("Enemy"))
		{
			EnemyBase scr = c.gameObject.GetComponent<EnemyBase>();
			scr.takeDamage(dmg);
			scr.damageTaken += dmg;
			if(hawkScript.enemiesToAttack.Contains(c.gameObject) == false)
			{
				hawkScript.enemiesToAttack.Add (c.gameObject);
			}

			Woodsman tempScr = woodsPlayer.GetComponent<Woodsman>();
			tempScr.hitCount+= 1;


			Destroy(gameObject);
		}
		else if(c.gameObject.CompareTag("wall"))
		{
			Destroy (gameObject);
		}
	}
}
