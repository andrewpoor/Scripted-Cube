using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhileScriptController : MonoBehaviour, IScriptController {

   public CommandDetails commandDetails; //Stores simple data about this command.
   public Transform bodyPiecePrefab; //Clones are to be instantiated as necessary.
   public GameObject foot; //The foot of the block.

   private List<GameObject> bodyPieces; //Indicators of the indentation level, and the block length. Doesn't include the bottom-most one.
   private List<GameObject> commands; //List of all commands. The header is also a command, but isn't in this list.
   public GameObject parentCommand; //The command above this one.

   //Boilerplate code for assembling the block's script.
   private string scriptHeader;
   private string scriptBody;
   private string scriptFooter;

   public void Start() {
      bodyPieces = new List<GameObject> ();
      commands = new List<GameObject> ();

      GetComponent<CommandDetails> ().numBlocks = 3;
   }

   public void InsertIntoScript(List<GameObject> surroundingCommands) {
      int positionOfTopmost = 0; //0 indicates no surrounding command is in the main script.

      foreach(GameObject command in surroundingCommands) {
         int otherPosition = command.GetComponent<CommandDetails> ().scriptPosition;

         if ((positionOfTopmost == 0) || (otherPosition > 0 && otherPosition < positionOfTopmost)) {
            positionOfTopmost = otherPosition;
            parentCommand = command;
            foot.GetComponent<FootCommandScriptController> ().parentCommand = command;
         }
      }

      if (positionOfTopmost > 0) {
         parentCommand.GetComponent<IScriptController>().AddCommand (gameObject, positionOfTopmost);
      }

      //When in the script, be ready to detect new commands being added below.
      foot.GetComponentInChildren<BoxCollider2D> ().enabled = true;
   }

   public void AddCommand(GameObject command, int positionOfAbove) {
      commands.Insert (positionOfAbove - commandDetails.scriptPosition, command);

      command.GetComponent<IScriptController>().SetScriptPosition(positionOfAbove + 1);
      command.GetComponent<CommandDetails> ().commandIndentation += 8.0f;
      command.transform.SetParent(transform, false);

      UpdateVisuals ();
   }

   public void RemoveFromScript() {
      if (commandDetails.scriptPosition > 0) {
         parentCommand.GetComponent<IScriptController>().RemoveCommand(gameObject, commandDetails.scriptPosition);
      }

      parentCommand = null;
      foot.GetComponent<FootCommandScriptController> ().parentCommand = null;

      //When not in the script, disable collision detection for the foot.
      foot.GetComponentInChildren<BoxCollider2D> ().enabled = false;
   }

   public void RemoveCommand(GameObject command, int positionOfThis) {
      commands.RemoveAt (positionOfThis - commandDetails.scriptPosition - 1);

      command.GetComponent<IScriptController>().SetScriptPosition(0);
      command.GetComponent<CommandDetails> ().commandIndentation = 0.0f;
      command.transform.SetParent(GetComponent<CommandDragController> ().commandsPane.transform, true);

      UpdateVisuals ();
   }

   public void UpdateParentCommand(GameObject newParentCommand) {
      parentCommand = newParentCommand;
      foot.GetComponent<IScriptController> ().UpdateParentCommand(newParentCommand);
   }

   //Called when setting external command chain, that is, independant of what's inside this block.
   public void UpdateChildsParentCommand(GameObject child) {
      foot.GetComponent<IScriptController> ().UpdateChildsParentCommand (child);
   }

   public void UpdateVisuals() {
      //Update block count.
      if (commands.Count == 0) {
         commandDetails.numBlocks = 3;
      } else {
         commandDetails.numBlocks = 2;

         foreach(GameObject command in commands) {
            commandDetails.numBlocks += command.GetComponent<CommandDetails> ().numBlocks;
         }
      }

      UpdateCommandParenting ();
      UpdatePositions ();
      UpdateScriptUI ();

      parentCommand.GetComponent<IScriptController> ().UpdateVisuals ();
   }

   public string CollateScript() {
      //Try to get the condition from the condition block. If it's not there, default to false as a condition.
      string condition = "false";

      //Iterate over immediate children to find the condition, if present.
      foreach(Transform child in transform) {
         IConditionScriptCode conditionCode = child.gameObject.GetComponent<IConditionScriptCode> ();
         if(conditionCode != null) {
            condition = conditionCode.GetCondition ();
         }
      }

      scriptHeader = @"
      while(" + condition + @") {
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

   //Set positions of both the header and the footer.
   public void SetScriptPosition(int position) {
      commandDetails.scriptPosition = position;
      foot.GetComponent<IScriptController> ().SetScriptPosition (position);
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

      //Move foot to bottom of script.
      foot.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0.0f, -blockHeight * Mathf.Max(blocks, 2));
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
