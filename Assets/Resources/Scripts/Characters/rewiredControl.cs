using UnityEngine;
using System.Collections;
using Rewired;

[AddComponentMenu("")]
[RequireComponent(typeof(CharacterController))]
public class rewiredControl : MonoBehaviour {
	
	public int playerId = 0; // The Rewired player id of this character
	
	public float moveSpeed = 5.5f;

	public int maxDist = 10; // Distance the players can be seperated from one another
	
	private Player player; // The Rewired Player
	private CharacterController cc;
	private Vector3 moveVector;
	private bool fire;
	private bool fireUp;
	private bool jump;
	private bool classAbility;
	private bool classAbilityUp;
	private bool utilityUp;
	private bool utilityDown;
	
	private static PlayerManager plyrMgr;
	
	private PlayerBase character;
	//for jumping and rotating torwards direction of travel
	//public float jumpForce = 0.50f;
	//public float verticalVelocity = 0.0f;
	public float rotationSpeed = 360.0f;
	
	[System.NonSerialized] // Don't serialize this so the value is lost on an editor script recompile.
	private bool initialized;
	
	void Awake() {
		cc = GetComponent<CharacterController>();
		character = GetComponent<PlayerBase>();
		playerId = character.playerNum;
		if(!plyrMgr)
		{
			plyrMgr = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
		}
	}
	
	private void Initialize() {
		// Get the Rewired Player object for this player.
		player = ReInput.players.GetPlayer(playerId);
		
		initialized = true;
	}
	
	void FixedUpdate () {
		if(!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
		if(!initialized) Initialize(); // Reinitialize after a recompile in the editor
		
		GetInput();
		ProcessInput();
		
		
	}
	
	private void GetInput() {
		// Get the input from the Rewired Player. All controllers that the Player owns will contribute, so it doesn't matter
		// whether the input is coming from a joystick, the keyboard, mouse, or a custom controller.
		
		moveVector.x = player.GetAxis("Move Horizontal"); // get input by name or action id
		moveVector.z = player.GetAxis("Move Vertical");
		fire = player.GetButtonDown("X");
		fireUp = player.GetButtonUp ("X");
		classAbility = player.GetButtonDown ("B");
		classAbilityUp = player.GetButtonUp ("B");
		jump = player.GetButtonDown ("A");
		//utilityUp = player.GetButtonUp ("Y");
		utilityDown = player.GetButtonDown ("Y");
	}
	
	private void ProcessInput() {
		moveVector.y = 0.0f;

		//Handle jumping and add it to the movement vector
		if (jump && !character.dead)
		{
			if(character.canJump)
			{
				//character.GetComponent<Animator>().SetTrigger("Jump");
				character.GetComponent<Animator>().SetBool("Jump", true);
				character.canJump = false;
				character.addForce(new Vector3(0.0f, character.jumpForce, 0.0f));
			}
		}
		else if (cc.isGrounded)
		{
			character.GetComponent<Animator>().SetBool("Jump", false);
			character.canJump = true;
			character.forces = new Vector3(character.forces.x, Mathf.Max(0.0f, character.forces.y), character.forces.z);
		}		

		character.addForce(new Vector3(0.0f, Physics.gravity.y * 2.0f * Time.deltaTime, 0.0f));

		if (!character.dead)
		{
			// Rotate the character to face in the direction that they will move
			if (new Vector3(moveVector.x, 0.0f, moveVector.z).magnitude > 0.0f)
			{
				transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (new Vector3 (moveVector.x, 0.0f, moveVector.z)), rotationSpeed * Time.deltaTime);
			}

			// Process fire button down
			if (fire) {
				Debug.Log("FIRE");
				character.basicAttack ("down");	
			}
			
			//process fire button up
			if(fireUp) {
				character.basicAttack("up");
			}
			
			//process class ability button down
			if (classAbility) {
				character.classAbility("down");
			}
			
			//process class ability button up
			if (classAbilityUp) {
				character.classAbility("up");
			}
		
			//if (utilityUp) 
			//{
			//Debug.Log("Need to implement Utility (Y) Button Up");
			//}
			
			if (utilityDown) 
			{
				character.itemAbility();
			}

			// Process movement
			if(moveVector.x != 0.0f || moveVector.z != 0.0f || moveVector.y != 0.0f) 
			{
				//if the player's distance from the group's center exceeds maxDist on the x or z
				//axis, they are stopped from moving on that axis. If they're already too far away,
				//they are only allowed to move closer to the center.
				Vector3 movement = moveVector * moveSpeed * Time.deltaTime * character.moveMulti;
				Vector3 oldDistVec = character.transform.position - plyrMgr.playersCenter;
				Vector3 newDistVec = oldDistVec + movement;

				float newX = Mathf.Abs(newDistVec.x);
				float oldX = Mathf.Abs(oldDistVec.x);

				float newZ = Mathf.Abs(newDistVec.z);
				float oldZ = Mathf.Abs(oldDistVec.z);

				if(newX > maxDist && newX > oldX)
				{
					movement.x = 0.0f;
				}
				if(newZ > maxDist && newZ > oldZ)
				{
					movement.z = 0.0f;
				}

				cc.Move(movement);
			}

			if(moveVector.magnitude > 0.0f)
			{
				character.GetComponent<Animator>().SetBool("Run", true);
			}
			else
			{
				character.GetComponent<Animator>().SetBool("Run", false);
			}
		}
	}
}
































