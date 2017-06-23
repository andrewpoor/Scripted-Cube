/* 
 * Handles movement of the condition when dragged by the mouse.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConditionDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
   public ConditionSpawner conditionSpawner; //Creates new commands when this one is moved.
   public Canvas canvas; //Regerence to the main canvas.
   public Image commandsPane; //Starting parent for commands.

   private bool onCommand = false; //Flag for position of the condition. It's either in the holder or attached to a command.
   private bool newCondition = true; //Flag for deciding whether a new condition needs to be spawned or not.

   //Collision detection with commands.
   private List<GameObject> touchingCommands;

   public void Start() {
      touchingCommands = new List<GameObject> ();
   }

   public void OnBeginDrag(PointerEventData eventData) {
      GetComponent<IColliderController> ().EnableCollider(true);
      conditionSpawner.transform.SetAsLastSibling (); //Bring to front.

      if(onCommand) {
         //Remove from command.
         transform.parent.gameObject.tag = "ConditionalCommand";
         transform.SetParent (commandsPane.transform, false);
      }
   }

   public void OnDrag(PointerEventData eventData) {
      Vector2 pos;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
      transform.position = canvas.transform.TransformPoint(pos);
   }

   public void OnEndDrag(PointerEventData eventData) {
      if (touchingCommands.Count > 0) {
         if (newCondition) {
            newCondition = false;

            //Spawn a new command back at the spawner.
            conditionSpawner.SpawnNewCondition ();
         }

         onCommand = true;
         AttachToTopCommand ();
         GetComponent<IColliderController> ().EnableCollider(false);

         //Reset list.
         touchingCommands = new List<GameObject> ();
      } else {
         GetComponent<IColliderController> ().EnableCollider(false);
         conditionSpawner.SpawnNewCondition (); //Spawn a new command back at the spawner.
         Destroy (gameObject); //Delete this command.
      }
   }

   public void OnTriggerEnter2D(Collider2D collider) {
      GameObject touchingCommand = collider.gameObject.transform.parent.gameObject;

      if(touchingCommand.CompareTag("ConditionalCommand")) {
         touchingCommands.Add (touchingCommand);
      }
   }

   public void OnTriggerExit2D(Collider2D collider) {
      GameObject touchingCommand = collider.gameObject.transform.parent.gameObject;

      if(touchingCommand.CompareTag("ConditionalCommand")) {
         touchingCommands.Remove (touchingCommand);
      }
   }

   //Choose the topmost command that this condition is touching and make it the parent.
   private void AttachToTopCommand() {
      int positionOfTopmost = 0;
      GameObject newParent = null;

      foreach(GameObject command in touchingCommands) {
         int otherPosition = command.GetComponent<CommandDetails> ().scriptPosition;

         if ((positionOfTopmost == 0) || (otherPosition > 0 && otherPosition < positionOfTopmost)) {
            positionOfTopmost = otherPosition;
            newParent = command;
         }
      }

      if (newParent != null) {
         transform.SetParent (newParent.transform, true);
         GetComponent<RectTransform> ().anchoredPosition = new Vector2 (109.0f, 0.0f);
         newParent.tag = "Untagged"; //Prevent the command from having new conditionals attached to it while this one is attached.
      }
   }
}
