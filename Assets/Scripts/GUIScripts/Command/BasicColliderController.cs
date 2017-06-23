using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicColliderController : MonoBehaviour, IColliderController {

   public BoxCollider2D boxCollider;

   private Vector2 fullSize;

   void Start() {
      fullSize = transform.parent.gameObject.GetComponent<RectTransform> ().sizeDelta;

      boxCollider = GetComponent<BoxCollider2D> ();
      boxCollider.size = fullSize;
      boxCollider.enabled = false;
   }

   //Make the collider smaller and position it at the top.
   public void ShrinkCollider() {
      boxCollider.size = Vector2.Scale(fullSize, new Vector2(1.0f, 0.5f));
      boxCollider.offset = new Vector2 (0.0f, fullSize.y / 4.0f);
   }

   //Make the collider full-sized and centred.
   public void ExpandCollider() {
      boxCollider.size = fullSize;
      boxCollider.offset = Vector2.zero;
   }

   public void SetSize(Vector2 size) {
      fullSize = size;

      if (boxCollider != null) {
         boxCollider.size = fullSize;
      }
   }

   public void EnableCollider(bool enabled) {
      boxCollider.enabled = enabled;
   }

   public void OnTriggerEnter2D(Collider2D collider) {
      transform.parent.gameObject.GetComponent<CommandDragController> ().HandleTriggerEnter (collider);
   }

   public void OnTriggerExit2D(Collider2D collider) {
      transform.parent.gameObject.GetComponent<CommandDragController> ().HandleTriggerExit (collider);
   }
}
