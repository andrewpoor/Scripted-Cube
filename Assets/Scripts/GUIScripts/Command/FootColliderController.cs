using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootColliderController : MonoBehaviour {

   public BoxCollider2D boxCollider;

   void Start() {
      boxCollider = GetComponent<BoxCollider2D> ();
      RectTransform rTransform = transform.parent.gameObject.GetComponent<RectTransform> ();
      boxCollider.size = new Vector2(rTransform.sizeDelta.x, rTransform.sizeDelta.y / 4.0f);
      boxCollider.offset = new Vector2(0.0f, -boxCollider.size.y * 1.5f);
      boxCollider.enabled = false;
   }
}
