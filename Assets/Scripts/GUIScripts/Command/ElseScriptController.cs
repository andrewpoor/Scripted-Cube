using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElseScriptController : MonoBehaviour, IScriptController {

   public CommandDetails commandDetails; //Stores simple data about this command.
   public Transform bodyPiecePrefab; //Clones are to be instantiated as necessary.

   private List<GameObject> bodyPieces; //Indicators of the indentation level, and the block length. Doesn't include the bottom-most one.
   private List<GameObject> commands; //List of all commands. The header is also a command, but isn't in this list.
   public IfElseScriptController ifElseHead; //The main part of the if/else block.
   public ElseColliderController elseCollider;

   //Boilerplate code for assembling the block's script.
   private string scriptHeader;
   private string scriptBody;
   private string scriptFooter;

   public void Start() {
      bodyPieces = new List<GameObject> ();
      commands = new List<GameObject> ();

      GetComponent<CommandDetails> ().numBlocks = 2;
   }

   //Shouldn't be called.
   public void InsertIntoScript(List<GameObject> surroundingCommands) {
      Debug.Log ("Warning: Trying to insert else part into script!");
   }

   public void AddCommand(GameObject command, int positionOfAbove) {
      commands.Insert (positionOfAbove - commandDetails.scriptPosition, command);

      command.GetComponent<IScriptController>().SetScriptPosition(positionOfAbove + 1);
      command.GetComponent<CommandDetails> ().commandIndentation += 8.0f;
      command.transform.SetParent(transform, false);

      UpdateVisuals ();
   }

   //Shouldn't be called.
   public void RemoveFromScript() {
      Debug.Log ("Warning: Trying to remove else part from script!");
   }

   public void RemoveCommand(GameObject command, int positionOfThis) {
      commands.RemoveAt (positionOfThis - commandDetails.scriptPosition - 1);

      command.GetComponent<IScriptController>().SetScriptPosition(0);
      command.GetComponent<CommandDetails> ().commandIndentation = 0.0f;
      command.transform.SetParent(ifElseHead.GetComponent<CommandDragController> ().commandsPane.transform, true);

      UpdateVisuals ();
   }

   //Shouldn't be called.
   public void UpdateParentCommand(GameObject newParentCommand) {
      Debug.Log ("Warning: Trying to update parent of an else part!");
   }

   //Shouldn't be called.
   public void UpdateChildsParentCommand(GameObject child) {
      Debug.Log ("Warning: Trying to set child's parent command in an else block!");
   }

   public void UpdateVisuals() {
      //Update block count.
      if (commands.Count == 0) {
         commandDetails.numBlocks = 2;
      } else {
         commandDetails.numBlocks = 1;

         foreach(GameObject command in commands) {
            commandDetails.numBlocks += command.GetComponent<CommandDetails> ().numBlocks;
         }
      }

      UpdateCommandParenting ();
      UpdatePositions ();
      UpdateScriptUI ();

      ifElseHead.UpdateVisuals ();
   }

   public string CollateScript() {
      scriptHeader = @"
      } else {
         yielded = false;
      ";

      scriptBody = "";
      foreach (GameObject command in commands) {
         scriptBody += command.GetComponent<IScriptController> ().CollateScript ();
      }

      scriptFooter = @"
         if(!yielded) {
            yield return null;
         }
      }
      ";

      return scriptHeader + scriptBody + scriptFooter;
   }

   public void SetScriptPosition(int position) {
      commandDetails.scriptPosition = position;
      UpdatePositions ();
   }

   //Shouldn't be called.
   public void LightUp() {
      Debug.Log ("Warning: Trying to light up a non-action!");
   }

   //Shouldn't be called.
   public void Unlight() {
      Debug.Log ("Warning: Trying to unlight a non-action!");
   }

   public void SetColliderEnabled(bool enabled) {
      elseCollider.boxCollider.enabled = enabled;
   }

   //Modifies the ordering position of each command in the list to account for any changes.
   private void UpdatePositions() {
      int position = commandDetails.scriptPosition + 1;
      foreach(GameObject command in commands) {
         command.GetComponent<IScriptController>().SetScriptPosition(position);
         command.transform.SetAsLastSibling ();
         position++;
      }
   }

   //Positions all of the commands to form a connected chain, adding body pieces to show indentation levels as appropriate.
   private void UpdateScriptUI() {
      //Reset indentation indicators.
      foreach(GameObject bodyPiece in bodyPieces) {
         Destroy (bodyPiece);
      }

      GameObject previous = gameObject; //The previous command.
      int blocks = 1; //How many blocks the script is long, so far, including the header.
      float blockHeight = gameObject.GetComponent<RectTransform> ().sizeDelta.y; //Height of a single block.

      //Position commands.
      foreach(GameObject command in commands) {
         float indentation = command.GetComponent<CommandDetails> ().commandIndentation;
         command.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (indentation, -blockHeight * blocks);

         previous = command;
         blocks += command.GetComponent<CommandDetails>().numBlocks;
      }

      //Add indentation indicators.
      for(int i = 2; i < blocks; i++) {
         //Create new indicator.
         GameObject bodyPiece = Instantiate(bodyPiecePrefab, Vector3.zero, transform.rotation).gameObject;
         bodyPiece.transform.SetParent (transform, false);
         bodyPieces.Add (bodyPiece);

         //Position indicator.
         bodyPiece.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0.0f, -blockHeight * i);
      }
   }

   //Set a parenting order in all of the commands. This is for signals that are passed up the chain recursively.
   private void UpdateCommandParenting() {
      GameObject previous = null;
      bool atHead = true;

      foreach(GameObject child in commands) {
         if(atHead) {
            child.GetComponent<IScriptController> ().UpdateParentCommand (gameObject);
            previous = child;
            atHead = false;
         } else {
            previous.GetComponent<IScriptController> ().UpdateChildsParentCommand (child);
            previous = child;
         }
      }
   }
}
