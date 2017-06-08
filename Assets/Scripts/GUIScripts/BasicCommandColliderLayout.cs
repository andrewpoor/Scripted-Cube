using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCommandColliderLayout : MonoBehaviour {

   void Start () {
      BoxCollider2D collider = GetComponent<BoxCollider2D> ();
      RectTransform rTransform = GetComponent<RectTransform> ();
      collider.size = rTransform.sizeDelta;
   }
}
