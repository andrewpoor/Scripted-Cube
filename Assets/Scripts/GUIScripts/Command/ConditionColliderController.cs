using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionColliderController : MonoBehaviour, IColliderController {

   private BoxCollider2D boxCollider;

   void Start() {
      boxCollider = GetComponent<BoxCollider2D> ();
      boxCollider.size = GetComponent<RectTransform> ().sizeDelta;
      boxCollider.enabled = false;
   }

   //Shouldn't be called.
   public void ShrinkCollider () {
      Debug.Log ("Trying to shrink collider of a condition!");
   }

   //Shouldn't be called.
   public void ExpandCollider() {
      Debug.Log ("Trying to shrink collider of a condition!");
   }

   public void EnableCollider(bool enabled) {
      boxCollider.enabled = enabled;
   }
}
