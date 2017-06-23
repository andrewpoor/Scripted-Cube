using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElseColliderController : MonoBehaviour {

   public BoxCollider2D boxCollider;

   void Start() {
      boxCollider = GetComponent<BoxCollider2D> ();
      boxCollider.size = Vector2.Scale(transform.parent.gameObject.GetComponent<RectTransform> ().sizeDelta, new Vector2(1.0f, 0.5f));
      boxCollider.offset = new Vector2 (0.0f, -boxCollider.size.y / 2.0f);
      boxCollider.enabled = false;
   }

   public void EnableCollider(bool enabled) {
      boxCollider.enabled = enabled;
   }
}
