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
   public CommandSpawner commandSpawner; //Creates new commands when this one is moved.
   public Canvas canvas; //Regerence to the main canvas.
   public Image commandsPane; //Starting parent for commands.

   private IScriptController scriptController; //Refernce to the script controller for this command.
   private bool inScript = false; //Flag for position of the command. It's either in the holder or the script.
   private bool newCommand = true; //Flag for deciding whether a new command needs to be spawned or not.

   //Collision detection with other commands.
   private List<GameObject> touchingCommands;

   public void Start() {
      touchingCommands = new List<GameObject> ();
      scriptController = GetComponent<IScriptController> ();
   }

   public void OnBeginDrag(PointerEventData eventData) {
      GetComponentInChildren<IColliderController> ().EnableCollider (true);
      GetComponentInChildren<IColliderController> ().ShrinkCollider ();
      commandSpawner.transform.SetAsLastSibling (); //Bring to front.
      touchingCommands = new List<GameObject> ();

      if(inScript) {
         scriptController.RemoveFromScript ();
      }
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
         scriptController.InsertIntoScript (touchingCommands);

         GetComponentInChildren<IColliderController> ().ExpandCollider ();

         //Reset list.
         touchingCommands = new List<GameObject> ();
      } else {
         GetComponentInChildren<IColliderController> ().EnableCollider(false); //Prevent the command from intefering with the newly-spawned one.
         commandSpawner.SpawnNewCommand (); //Spawn a new command back at the spawner.
         Destroy (gameObject); //Delete this command.
      }
   }

   public void HandleTriggerEnter(Collider2D collider) {
      touchingCommands.Add (collider.gameObject.transform.parent.gameObject);
   }
  
   public void HandleTriggerExit(Collider2D collider) {
      touchingCommands.Remove (collider.gameObject.transform.parent.gameObject);
   }
}
