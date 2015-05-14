// Version 1.3.9
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace Exploder
{
    /// <summary>
    /// this is an optional script
    /// if you don't want to tag your GameObject by "Exploder" you can assign this script to your GameObject instead
    /// use this if your GameObject has already a tag and you don't want to re-tag it
    /// </summary>
    public class Explodable : MonoBehaviour
    {
		public ExploderObject exploder = null;

		public float force = 1.0f;
		public float radius = 0.5f;
		public int targetFragments = 10;
		public Vector3 ForceVector;

		void Start () {
			exploder = GameObject.FindGameObjectWithTag("TheExploder").GetComponent<ExploderObject>();
			ForceVector = -Camera.main.transform.up.normalized;
			
		}
		
		void Boom(){
			var centroid = ExploderUtils.GetCentroid(gameObject);
			
			// place the exploder object to centroid position
			exploder.transform.position = centroid;
			exploder.ExplodeSelf = false;
			
			// adjust force vector to be in direction from shotgun
			exploder.ForceVector = ForceVector;
			//                Utils.Log("ForceVec: " + exploder.ForceVector);
			exploder.Force = force;
			exploder.UseForceVector = true;
			
			// fragment pieces
			exploder.TargetFragments = targetFragments;
			
			// set explosion radius to 5 meters
			exploder.Radius = radius;
			
			// run explosion
			exploder.Explode();

			//Destroy (this);
			
			
		}

    }
}
