using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadController : MonoBehaviour {

   public List<GameObject> bodyPieces; //Indicators of the indentation level, and the script length. Doesn't include the bottom-most one.
   public GameObject lastBodyPiece; //The bottom-most indentation indicator.
   public Transform bodyPiece; //To be instantiated when needed.

   public GameObject connector; //The connector for commands to attach to.

   private int numBlocks = 0; //The number of blocks in the script; this is for accounting for the size of the graphical representation.

   public void Start() {
      bodyPieces = new List<GameObject> ();
   }

   //Called by child when it becomes attached to this command.
   public void NewCommandAdded(int blocks) {
      connector.SetActive (false);

      int extraBlocks = 0; //How many extra blocks are being added.

      if (numBlocks == 0) {
         extraBlocks = blocks - 1;
      } else {
         extraBlocks = blocks;
      }

      for(int i = 0; i < extraBlocks; i++) {
         Transform cloneBody = Instantiate (bodyPiece, transform.position, Quaternion.identity, transform);
         RectTransform rTClone = cloneBody.gameObject.GetComponent<RectTransform> ();
         rTClone.anchoredPosition = new Vector2 (0.0f, -(numBlocks + i) * rTClone.sizeDelta.y);
      }

      if (numBlocks == 0) {
         numBlocks = blocks;
      } else {
         numBlocks += blocks;
      }

      RectTransform rTL = lastBodyPiece.GetComponent<RectTransform> ();
      rTL.anchoredPosition = new Vector2 (0.0f, -(numBlocks) * rTL.sizeDelta.y);
   }
}
