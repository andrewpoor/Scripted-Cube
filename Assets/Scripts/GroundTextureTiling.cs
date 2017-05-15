using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTextureTiling : MonoBehaviour {

	// Use this for initialization
	void Start () {
      Renderer renderer = GetComponent<Renderer> ();
      renderer.material.mainTextureScale = new Vector2 (transform.lossyScale.x, transform.lossyScale.y);
	}
}
