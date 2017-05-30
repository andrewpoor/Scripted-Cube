using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScreenSizeController : MonoBehaviour {

   public float xScale = 0.5f;
   public float yScale = 0.5f;

	// Use this for initialization
	void Start () {
      Resolution resolution = Screen.currentResolution;

      //transform = new Vector3 (resolution.width * xScale, resolution.height * yScale, 1.0f);

      GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width * xScale, Screen.height * yScale);
	}
}
