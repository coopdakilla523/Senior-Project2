/*using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthScaling : MonoBehaviour {
	
	public RawImage healthBar;

	void Start() {
		Debug.Log (healthBar.rectTransform.sizeDelta);
	}

	// Update is called once per frame
	void Update () {

		if (healthBar.rectTransform.sizeDelta.x > -322) {
			healthBar.rectTransform.sizeDelta = healthBar.rectTransform.sizeDelta - (new Vector2 (2.0f, 0.0f));
			Debug.Log(healthBar.rectTransform.sizeDelta);
		}


	}
}
*/