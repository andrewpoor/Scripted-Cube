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
   public Canvas canvas; //Regerence to the main canvas.

   //Positioning of command.
   private RectTransform rTransform; //Positioning of the command.
   private Vector2 pointerOffset; //Offset of the mouse when dragging.
   private Vector2 canvasScale; //To account for different resolutions.

   private BoxCollider2D boxCollider;
   private bool inScript = false; //Flag for position of the command. It's either in the holder or the script.
   private bool newCommand = true; //Flag for deciding whether a new command needs to be spawned or not.

   //Collision detection with other commands.
   private List<GameObject> touchingCommands;

   public void Start() {
      rTransform = GetComponent<RectTransform> ();
      touchingCommands = new List<GameObject> ();

      boxCollider = GetComponent<BoxCollider2D> ();
      boxCollider.enabled = false;

      canvasScale = new Vector2 (canvas.GetComponent<CanvasScaler>().referenceResolution.x / Screen.width, canvas.GetComponent<CanvasScaler>().referenceResolution.y / Screen.height);
   }

   public void OnBeginDrag(PointerEventData eventData) {
      boxCollider.enabled = true;

      if(inScript) {
         commandScriptController.RemoveFromScript ();
      }

      pointerOffset = rTransform.anchoredPosition - eventData.position;
      //pointerOffset = Vector2.Scale (pointerOffset, canvasScale);
   }

   public void OnDrag(PointerEventData eventData) {
      Vector2 pos;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
      transform.position = canvas.transform.TransformPoint(pos);

      //rTransform.anchoredPosition = eventData.position + pointerOffset;
      //rTransform.anchoredPosition = Vector2.Scale (rTransform.anchoredPosition, canvasScale);
   }

   public void OnEndDrag(PointerEventData eventData) {
      if (touchingCommands.Count > 0) {
         if (newCommand) {
            newCommand = false;

            //Spawn a new command back at the spawner.
            commandSpawner.SpawnNewCommand ();
         }

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
