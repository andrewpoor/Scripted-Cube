/*
 * Represents the command at a UI level, bar script placement.
 * 
 * Handles movement of the command when dragged by the mouse.
 * Calls the command's script controller as appropriate when docking into the script.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
   public CommandScriptController commandScriptController; //Refernce to the script controller for this command.
   public CommandSpawner commandSpawner; //Creates new commands when this one is moved.

   //Positioning of command.
   private RectTransform rTransform; //Positioning of the command.
   private Vector2 pointerOffset; //Offset of the mouse when dragging.

   private BoxCollider2D boxCollider;
   private bool inScript = false; //Flag for position of the command. It's either in the holder or the script.

   //Collision detection with other commands.
   private List<GameObject> touchingCommands;

   public void Start() {
      rTransform = GetComponent<RectTransform> ();
      touchingCommands = new List<GameObject> ();

      boxCollider = GetComponent<BoxCollider2D> ();
      boxCollider.enabled = false;
   }

   public void OnBeginDrag(PointerEventData eventData) {
      boxCollider.enabled = true;

      if(inScript) {
         commandScriptController.RemoveFromScript ();
      }

      pointerOffset = rTransform.anchoredPosition - eventData.position;
   }

   public void OnDrag(PointerEventData eventData) {
      rTransform.anchoredPosition = eventData.position + pointerOffset;
   }

   public void OnEndDrag(PointerEventData eventData) {
      if (touchingCommands.Count > 0) {
         //Spawn a new command back at the spawner.
         commandSpawner.SpawnNewCommand ();

         inScript = true;

         commandScriptController.InsertIntoScript (touchingCommands);

         //Reset list.
         touchingCommands = new List<GameObject> ();
      } else {
         boxCollider.enabled = false; //Prevent the command from intefering with the newly-spawned one.
         commandSpawner.SpawnNewCommand (); //Spawn a new command back at the spawner.
         Destroy (gameObject); //Delete this command.
      }
   }

   public void OnTriggerEnter2D(Collider2D collider) {
      touchingCommands.Add (collider.gameObject);
   }
  
   public void OnTriggerExit2D(Collider2D collider) {
      touchingCommands.Remove (collider.gameObject);
   }
}
