using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockColliderController : MonoBehaviour, IColliderController {

   private BoxCollider2D boxCollider;
   private Vector2 fullSize;
   private float xOffset = -70.0f;

   void Start() {
      fullSize = new Vector2(160.0f, 50.0f);

      boxCollider = GetComponent<BoxCollider2D> ();
      boxCollider.size = fullSize;
      boxCollider.offset = new Vector2 (xOffset, boxCollider.offset.y);
      boxCollider.enabled = false;
   }

   //Make the collider smaller and position it at the top.
   public void ShrinkCollider() {
      boxCollider.size = Vector2.Scale(fullSize, new Vector2(1.0f, 0.5f));
      boxCollider.offset = new Vector2 (xOffset, fullSize.y / 4.0f);
   }

   //Make the collider full-sized and centred.
   public void ExpandCollider() {
      boxCollider.size = fullSize;
      boxCollider.offset = new Vector2 (xOffset, 0.0f);
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
