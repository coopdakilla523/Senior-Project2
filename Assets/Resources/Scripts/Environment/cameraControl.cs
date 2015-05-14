using UnityEngine;
using System.Collections;
using System;

public class cameraControl : MonoBehaviour
{	
	private bool isOrthographic;

    public GameObject[] targets;
	public GameObject captureBox;
	public float[] angleClamp = {15, 8};
	public float maxDistanceAway = 40f;
	public Vector3 avgDistance;
	public float playerHight = 2f;
	public float bufferSize = 4f;
	public bool debugBool = false;
	public float xRatio = .8888f;
	public float hightMin = 20;
	public float rotationAngle = 15;

	public float zTweek = 0;
	public float yTweek = 0;

	private float horFoV;
	private float virFoV;
	private const float radConversion = (float)(Math.PI / 180);

    // Starts the camera, gets the player targest, and gets the diagonal distance of the room
    public void Start ()
    {
        Console.WriteLine ("START");
        targets = GameObject.FindGameObjectsWithTag ("Player");
        if (Camera.main) {
            isOrthographic = Camera.main.orthographic;
        }
		
		virFoV = Camera.main.fieldOfView * radConversion;
		horFoV = 2 * Mathf.Atan(Camera.main.aspect * Mathf.Tan((float)(virFoV)));

		Debug.Log (Camera.main.aspect);
    }

	public void FixedUpdate() 
	{
		Console.WriteLine ("START");
		// If we can't find a player, we return without altering the camera's position
		if (!GameObject.FindWithTag ("Player")) {
			return;
		}

		// We get information on the two players that are the farthest apart
		float[] seperationInfo = LargestDifferenceInfo ();
		float largestDifference = seperationInfo [0];
		float xMax = seperationInfo[1] + bufferSize;
		float zMax = seperationInfo[2] + bufferSize;
		float xMid = seperationInfo[3];
		float zMid = seperationInfo[4];

		/* We need to set the camera to contain all players. We use the information gained from the last call
		 * First, we use some trig nonsense to get the horizontal FoV, and also get the offset angle for the camera.
		 * Then, we check if its the x or z distance that is determining how much we need on camera. 
		 * If its the x length, we take what we alerady have found and simply finish by finding the hight. However
		 * if its the z, we redo the calculations using equations based on z being the bounding axis. 
		 * 
		 * The work and proofs for the calculations can be found in the SVN.
		 */

		// Players largest distance between one another divided by the max distance they can be apart
		float distanceRatio = largestDifference / maxDistanceAway;

		xMax += xMax *  (2 - distanceRatio) * xRatio;
		// Angle the camera will be shifted
		float shiftAngle = (float)(Mathf.LerpAngle (angleClamp [0], angleClamp [1], distanceRatio)); 
		shiftAngle = shiftAngle * radConversion;

		float yOffset = ((xMax * .5f) / (Mathf.Tan (horFoV/2f))) + playerHight;
		float zOffset = yOffset * Mathf.Tan (shiftAngle);
		float constrainedZ = Mathf.Tan (shiftAngle + virFoV) * (yOffset - playerHight) - zOffset;

		if (constrainedZ < zMax) {
			//Debug.Log ("Bound by Z Axis");
			constrainedZ = zMax;
			yOffset = ((Mathf.Tan (shiftAngle + virFoV) * playerHight + zMax) / (Mathf.Tan(shiftAngle + virFoV) - Mathf.Tan (shiftAngle)));
			zOffset = yOffset * Mathf.Tan(shiftAngle);
		}

		if (yOffset < hightMin) 
		{
			yOffset = hightMin;
			zOffset = yOffset * Mathf.Tan (shiftAngle);
			constrainedZ = Mathf.Tan (shiftAngle + virFoV) * (yOffset - playerHight) - zOffset;
		}
	
		Vector3 newCamPos;
		if (Time.deltaTime >= 1)
		{
			newCamPos = new Vector3 (xMid + (yOffset * .2f), yOffset + zTweek, zMid + zOffset + constrainedZ / 2 + zTweek);
		}
		else 
		{
//			Debug.Log(new Vector3 (xMid, yOffset, zMid + zOffset + constrainedZ / 2));
//			Debug.Log (Camera.main.transform.position);
			float newX = Camera.main.transform.position.x - 3 * Time.deltaTime * (Camera.main.transform.position.x - (xMid + (yOffset * .2f)));
			float newY = Camera.main.transform.position.y - 3 * Time.deltaTime * (Camera.main.transform.position.y - yOffset + yTweek);
			float newZ = Camera.main.transform.position.z - 3 * Time.deltaTime * (Camera.main.transform.position.z - (zMid + zOffset + constrainedZ / 2) + zTweek);
			newCamPos = new Vector3 (newX, newY + yTweek, newZ + zTweek);
		}
		//newCamPos = edgeShiftCheck(newCamPos);
		//Camera.main.transform.position = rotation * newCamPos;

		//Camera.main.transform.position = new Vector3 (xMid, yOffset, zMid + zOffset + constrainedZ / 2);
		//Camera.main.transform.LookAt(new Vector3 (xMid, 0, zMid));
		Camera.main.transform.eulerAngles = new Vector3 (90 - Camera.main.fieldOfView / 2 - shiftAngle / radConversion, 180 + rotationAngle, shiftAngle);
		Camera.main.transform.position = newCamPos;
		//Debug.Log ("Camera.main.transform.position: " + Camera.main.transform.position + " rotation * newCamPos: " + rotation * newCamPos);

		checkForHiddenCharacters ();

		// DEBUG STUFF
		if (debugBool) 
			debug(shiftAngle, largestDifference, xMax, zMax, xMid, zMid, zOffset, yOffset);
	}

    // Gets the largest distance between any two players, and returns it
    public float[] LargestDifferenceInfo ()
    {
        float currentDistance = 0.0f;
		float curXMax = 0.0f;
		float curZMax = 0.0f;
        float largestDistance = 0.0f;
		float xMax = 0.0f;
		float zMax = 0.0f;
		float xMid = targets[0].transform.position.x;  
		float zMid = targets[0].transform.position.z;

        for (int i = 0; i < targets.Length - 1; i++) {
            for (int j = i + 1; j < targets.Length; j++) {
				Vector3 temp1 = new Vector3(targets[i].transform.position.x, 0, targets[i].transform.position.z);
				Vector3 temp2 = new Vector3(targets[j].transform.position.x, 0, targets[j].transform.position.z);
				currentDistance = Vector3.Distance(temp1, temp2);
				curXMax = Mathf.Abs(targets[i].transform.position.x - targets[j].transform.position.x); 
				curZMax = Mathf.Abs(targets[i].transform.position.z - targets[j].transform.position.z);
                if (currentDistance > largestDistance) {
					largestDistance = currentDistance;
                }
				if (xMax < curXMax)
				{
					xMax = curXMax;
					xMid = (targets[i].transform.position.x + targets[j].transform.position.x) / 2;
				}
				if (zMax < curZMax)
				{
					zMax = curZMax;
					zMid = (targets[i].transform.position.z + targets[j].transform.position.z) / 2;
				}
            }
        }

		float[] toReturn = {largestDistance, xMax, zMax, xMid, zMid};
		return toReturn;
    }

	public Vector3 edgeShiftCheck (Vector3 cameraPos) 
	{
		RaycastHit hit;
		Vector3 shiftedPos = new Vector3(cameraPos.x, 1, cameraPos.z);
		float forwardWall = 0;
		float leftWall = 0;
		float backWall = 0;
		float rightWall = 0;
		float detectionLength = 5.0f;
		int layerMask = 1 << 14;
		bool updateNeeded = false;
		
		//Debug.DrawRay(shiftedPos, Vector3.forward, Color.red);
		Debug.DrawRay(shiftedPos, Vector3.left * detectionLength, Color.red);
		Debug.DrawRay(shiftedPos, Vector3.right * detectionLength, Color.red);
		//Debug.DrawRay(shiftedPos, Vector3.back, Color.red);

		/*if (Physics.Raycast(shiftedPos, Vector3.forward, out hit, detectionLength, layerMask)) 
		{
			forwardWall = Vector3.Distance(hit.point, shiftedPos);
			forwardWall = 2.0f - forwardWall;
			updateNeeded = true;
		} */
		if (Physics.Raycast(shiftedPos, Vector3.left, out hit, detectionLength, layerMask)) 
		{
			leftWall = Vector3.Distance(hit.point, shiftedPos);
			leftWall = detectionLength - leftWall;
			updateNeeded = true;
		}
		/*if (Physics.Raycast(shiftedPos, Vector3.back, out hit, detectionLength, layerMask)) 
		{
			backWall = Vector3.Distance(hit.point, shiftedPos);
			backWall = 2.0f - backWall;
			updateNeeded = true;
		} */
		if (Physics.Raycast(shiftedPos, Vector3.right, out hit, detectionLength, layerMask)) 
		{
			rightWall = Vector3.Distance(hit.point, shiftedPos);
			rightWall = detectionLength - rightWall;
			updateNeeded = true;
		}
		
		if (updateNeeded) 
		{
			cameraPos = new Vector3(cameraPos.x + leftWall - rightWall, cameraPos.y, cameraPos.z + backWall - forwardWall);
			Debug.Log("OLD CAM: x-pos: " + cameraPos.x + " y-pos: " + cameraPos.y + " z-pos: " + cameraPos.z);
			Debug.Log("Shift: leftWall: " + leftWall + " rightWall: " + rightWall + " backWall: " + backWall + " forwardWall: " + forwardWall); 
		}

		return cameraPos;
	}

	//public GameObject cube;
	private void debug (float angle, float longest, float xMax, float zMax, float xMid, float zMid, float zOffset, float yOffset)
	{
		Debug.DrawLine (Camera.main.transform.position, new Vector3 (xMid, 0, zMid));
		Debug.Log ("DRAW CALL RESULTS");
		Debug.Log ("angleOffset: " + angle.ToString());
		Debug.Log ("longest: " + longest.ToString());
		Debug.Log ("xMax: " + xMax.ToString());
		Debug.Log ("zMax: " + zMax.ToString());
		Debug.Log ("xMid: " + xMid.ToString());
		Debug.Log ("zMid: " + zMid.ToString());
		Debug.Log ("zOffset: " + zOffset.ToString());
		Debug.Log ("yOffset: " + yOffset.ToString() + "\n");
		//Debug.Log ("FoV: " + (Camera.main.fieldOfView).ToString());
		//cube.transform.position = new Vector3 (xMid,1, zMid);
		//cube.transform.localScale = new Vector3 (xMax, 2, zMax);
		//Debug.Break();
	}

	private void checkForHiddenCharacters ()
	{
		RaycastHit hit;
		int onlyWalls = 1 << 14;
		for (int i = 0; i < targets.Length; i++) 
		{
			if (Physics.Linecast(Camera.main.transform.position, targets[i].transform.position, out hit, onlyWalls)) {
				Debug.Log("Someone's behind a wall!");
			}
		}
	}

}


