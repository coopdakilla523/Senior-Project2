using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(CharacterController))]

public class cameraControl2 : MonoBehaviour
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

		captureBox = GameObject.Find ("CaptureBox");

		Debug.Log (Camera.main.aspect);
    }

	public void LateUpdate() 
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
	
		if (Time.deltaTime >= 1)
		{
			Camera.main.transform.position = new Vector3 (xMid, yOffset, zMid + zOffset + constrainedZ / 2);
		}
		else 
		{
//			Debug.Log(new Vector3 (xMid, yOffset, zMid + zOffset + constrainedZ / 2));
//			Debug.Log (Camera.main.transform.position);
			float newX = Camera.main.transform.position.x - Time.deltaTime * (Camera.main.transform.position.x - xMid);
			float newY = Camera.main.transform.position.y - Time.deltaTime * (Camera.main.transform.position.y - yOffset);
			float newZ = Camera.main.transform.position.z - Time.deltaTime * (Camera.main.transform.position.z - (zMid + zOffset + constrainedZ / 2));
			Camera.main.transform.position = new Vector3 (newX, newY, newZ);
		}
		//Camera.main.transform.position = new Vector3 (xMid, yOffset, zMid + zOffset + constrainedZ / 2);
		//Camera.main.transform.LookAt(new Vector3 (xMid, 0, zMid));
		Camera.main.transform.eulerAngles = new Vector3 (90 - Camera.main.fieldOfView / 2 - shiftAngle / radConversion, 180, 0);
		captureBox.transform.position = new Vector3(xMid, 0, zMid);

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
		Debug.Log (targets);
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

	
	//public GameObject cube;
	private void debug(float angle, float longest, float xMax, float zMax, float xMid, float zMid, float zOffset, float yOffset)
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

}

// This is the old code used in the previous version of the camera. Its kept in case we need to refer back on our
// previous methods for new ideas, or alterations to the current code. 

/* EVEN LESS OLD CODE
 * 
 * public void LateUpdateOLD ()
    {
        // If we can't find a player, we return without altering the camera's position
        if (!GameObject.FindWithTag ("Player")) {
            return;
        }

		// We sum the positions of all of the players, and from there we find the mid point between all of them
		Vector3 sum = new Vector3 (0, 0, 0);
		for (int i = 0; i < targets.Length; i++) {
			sum += targets [i].transform.position;
		}
		avgDistance = sum / targets.Length;
		captureBox.transform.position = avgDistance;
		
		// Next, we find what the biggest difference in distance between any two characters is
		float[] largestDifferenceInfo = LargestDifferenceInfo ();
		float largestDifference = largestDifferenceInfo [0];
		float maxX = largestDifferenceInfo [1];
		float maxZ = largestDifferenceInfo [2];

		float distanceOffset = largestDifference / maxDistanceAway;
		float yOffset = Mathf.Lerp (yClamp [0], yClamp [1], distanceOffset);
		float zOffset = Mathf.Lerp (zClamp [0], zClamp [1], distanceOffset);

		// The camera is set to its new position, pointed to the point to look at. 
		Camera.main.transform.position = new Vector3 (avgDistance.x, yOffset, avgDistance.z + zOffset);
		Camera.main.transform.LookAt(avgDistance);

                
    }
 *
 *
 */

/* LESS OLD CODE
 * 
 * private bool isOrthographic;
    public GameObject[] targets;
    //public GameObject avgBox;
    public float currentDistance;
    public float largestDistance;
    public float height = 5.0f;
    public Vector3 avgDistance;
    public float distance = 5.0f;                    // Default Distance 
    public float speed = 1;
    public float offset;
    public float roomLength = 48f;
    private float roomDiagonal;
    public float xRatio = 0f;
    public float yzRatio = 0f;
    public float offZRatio = 0f;
    public float offXRatio = 0f;
    public float[] xClamp = {-8,8};
    public float[] yClamp = {10,30};
    public float[] zClamp = {7,27};
    public float[] offsetClamp = {10, -10, 10, -10};
	public bool dubugText = true;
 * 
        // We sum the positions of all of the players, and from there we find the mid point between all of them
        Vector3 sum = new Vector3 (0, 0, 0);
        for (int i = 0; i < targets.Length; i++) {
            sum += targets [i].transform.position;
        }
        avgDistance = sum / targets.Length;

        // Next, we find what the biggest difference in distance between any two characters is
        float largestDifference = returnLargestDifference ();
                
        // The camera is done via clamping. The positions of the player weights how big of an offset
        // is used. This allows us to zoom in and out, and shift the camera to the left and right
        // to keep the room mostly in room. 

        // The xRatio of the room is what decides how far to the left or right the camera will shift
        // It is the current position of the players center divided by the rooms length
        xRatio = (avgDistance.x / roomLength) + .5f;
        float tempX = avgDistance.x + Mathf.Lerp (xClamp [0], xClamp [1], xRatio);

        // The yzRatio is the largest difference between two characters divided by the length. This is
        // used to decide the position of the Y, and Z of the camera depending on exactly where the players are
        yzRatio = largestDistance / roomLength;
        float tempY = Mathf.Lerp (yClamp [0], yClamp [1], yzRatio);
        float tempZ = avgDistance.z + 5 + Mathf.Lerp (zClamp [0], zClamp [1], yzRatio);

        // The offZRatio decides the exact position where the camera looks at. This lets the camera not always look
        // dead center, which is sometimes needed to finetune the exacts of the camera
        offZRatio = (avgDistance.z / roomLength) + .5f;
        float lookZOffset = Mathf.Lerp (offsetClamp [0], offsetClamp [1], offZRatio);
        float lookXOffset = Mathf.Lerp (offsetClamp [2], offsetClamp [3], xRatio);

        // The camera is set to its new position, pointed to the point to look at. 
        Camera.main.transform.position = new Vector3 (tempX, tempY, tempZ);
        Camera.main.transform.LookAt(new Vector3 ((avgDistance.x + lookXOffset), avgDistance.y, (avgDistance.z + lookZOffset)));


        // 'AVG BOX' code is simply debug code that's useful when trying to figure out how the camera is working. It 
        // is set to be where ever the camera is looking at. 
        //avgBox.transform.position = new Vector3 ((avgDistance.x + lookXOffset), avgDistance.y, (avgDistance.z + lookZOffset));

// All the GUI debug info
    public void OnGUI ()
    {
        GUILayout.Label ("largest distance is = " + largestDistance.ToString ());
        GUILayout.Label ("height = " + height.ToString ());
        GUILayout.Label ("number of players = " + targets.Length.ToString ());
        GUILayout.Label ("xRatio = " + xRatio.ToString ());
        GUILayout.Label ("yzRatio = " + yzRatio.ToString ());
        GUILayout.Label ("offRatio = " + offZRatio.ToString ());
        GUILayout.Label ("offRatio = " + offXRatio.ToString ());
		GUILayout.Label ("Average Distance = " + avgDistance.x.ToString () + ", " + avgDistance.y.ToString () + ", " + avgDistance.z.ToString ());
	}

 */






/* OLD CODE
 * targets = GameObject.FindGameObjectsWithTag ("Player");
        if (!GameObject.FindWithTag ("Player")) {
            return;
        }
        Vector3 sum = new Vector3 (0, 0, 0);
        for (int i = 0; i < targets.Length; i++) {
            sum += targets [i].transform.position;
        }

        avgDistance = sum / targets.Length;

        float largestDifference = returnLargestDifference ();

        distance = Mathf.Lerp (height, largestDifference, 0.5f);

        if (largestDifference < 8) {
            distance = 8;
            Vector3 temp = new Vector3 (theCamera.transform.position.x, 10, theCamera.transform.position.z);
            theCamera.transform.position = temp;
        }

        if (largestDifference > 8) {
            distance = 8;
        }
        */