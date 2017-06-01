using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

   public GameObject concaveConnector;
   public GameObject convexConnectorDummy; //Empty object to set collider transform.

   public float commandIndentation;

   private GameObject parentCommand; //The command that may become the parent to this one, that is, the previous command in the chain.
   private BoxCollider2D boxCollider; //Collider for the convex connector.
   private RectTransform rTransform;
   private Vector2 pointerOffset; //Offset of the mouse when dragging.
   private bool parented = false; //True if connected to the main script.

   public void Start() {
      boxCollider = GetComponent<BoxCollider2D> ();
      boxCollider.offset = convexConnectorDummy.GetComponent<RectTransform>().anchoredPosition;
      boxCollider.size = convexConnectorDummy.GetComponent<RectTransform>().sizeDelta;

      BoxCollider2D concaveCollider = concaveConnector.GetComponent<BoxCollider2D> ();
      concaveCollider.size = concaveConnector.GetComponent<RectTransform>().sizeDelta;

      rTransform = GetComponent<RectTransform> ();
   }

   public void OnBeginDrag(PointerEventData eventData) {
      pointerOffset = rTransform.anchoredPosition - eventData.position;
   }

   public void OnDrag(PointerEventData eventData) {
      rTransform.anchoredPosition = eventData.position + pointerOffset;
   }

   public void OnEndDrag(PointerEventData eventData) {
      if (parentCommand != null) {
         parented = true;

         float indentation = 0.0f;
         if (parentCommand.CompareTag ("AddIndentation")) {
            indentation = commandIndentation;
         }

         //Lock to position beneath parent command.
         transform.SetParent (parentCommand.transform, false);
         rTransform.anchorMax = new Vector2 (0.5f, 0.0f);
         rTransform.anchorMin = new Vector2 (0.5f, 0.0f);
         rTransform.pivot = new Vector2 (0.5f, 0.5f);
         rTransform.anchoredPosition = new Vector2 (indentation, 0.0f);

         //Update indentation indicators as appropriate.
         parentCommand.SendMessage ("NewCommandAdded", 1);
      } else {
         parented = false;
      }
   }

   public void OnTriggerEnter2D(Collider2D collider) {
      if (collider.gameObject.CompareTag ("ConcaveConnector") && !parented) {
         parentCommand = collider.gameObject.transform.parent.gameObject;
      }
   }

   public void OnTriggerExit2D(Collider2D collider) {
      if (collider.gameObject.CompareTag ("ConcaveConnector") && !parented) {
         parentCommand = null;
      }
   }

   //Called by child when it becomes attached to this command.
   public void NewCommandAdded(int numBlocks) {
      if(parentCommand == null) {
         Debug.Log ("NULL HERE.");
         return;
      }
      //Propogate message.
      parentCommand.SendMessage("NewCommandAdded", numBlocks);
   }
}
