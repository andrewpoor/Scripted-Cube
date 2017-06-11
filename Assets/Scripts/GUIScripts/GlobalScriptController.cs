using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalScriptController : MonoBehaviour {

   public GameObject commandsPane; //Removed commands are parented to here.
   public Transform bodyPiecePrefab; //Clones are to be instantiated as necessary.
   public GameObject foot; //The foot of the script.

   public float tileTime = 1.0f; //Time taken for the player to move one tile.
   public float spinTime = 1.0f; //Time taken for the player to rotate 90 degrees.

   private List<GameObject> bodyPieces; //Indicators of the indentation level, and the script length. Doesn't include the bottom-most one.
   private List<GameObject> commands; //List of all commands. The header is also a command, but isn't in this list.

   //Boilerplate code for assembling the user's script.
   private string scriptHeader;
   private string scriptBody;
   private string scriptFooter;

   private string loopIndexIdentifier = ""; //Dynamically updates to create new identifiers.

   public void Start() {
      bodyPieces = new List<GameObject> ();
      commands = new List<GameObject> ();

      BoxCollider2D collider = GetComponent<BoxCollider2D> ();
      collider.size = GetComponent<RectTransform> ().sizeDelta;

      scriptHeader = @"
using UnityEngine;
using System.Collections;
using System;

public class ScriptedPlayerController : DynamicPlayerController
{
   public IEnumerator GetCoroutine(PlayerController parent) {
      return CustomScript(parent);
   }

   public IEnumerator CustomScript(PlayerController parent) {
   float timer = 0.0f;
   float totalTime = 0.0f;
   int distance = 0;
   int rotations = 0;
   float tileTime = " + tileTime + @"f;
   float spinTime = " + spinTime + @"f;
   Vector3 startPosition;
   Vector3 targetPosition;
   Quaternion startRotation;
   Quaternion targetRotation;
   bool yielded;
   ";

      scriptBody = "";

      scriptFooter = @"
   yield return null;
   }
}
   ";
   }

   //Called by children when a new command is added.
   //Adds the command to the chain of commands and updates the visuals appropriately.
   public void AddCommand(GameObject newCommand, int previousScriptPosition) {
      //The first command in the list woould be position 2, as the script header is position 1. 
      //Previous position is -1, -1 again for -2 as required for a position of 2 to be at index 0 in the list.
      commands.Insert (previousScriptPosition - 1, newCommand);

      newCommand.GetComponent<CommandDetails> ().scriptPosition = previousScriptPosition + 1;
      newCommand.GetComponent<CommandDetails> ().commandIndentation += 8.0f;
      newCommand.transform.SetParent(transform, false);

      UpdatePositions ();
      UpdateScriptUI ();
   }

   //Called by children when a command is to be removed from the list.
   //Removes the command from the chain of commands and updates the visuals appropriately.
   public void RemoveCommand(GameObject oldCommand, int oldPosition) {
      commands.RemoveAt (oldPosition - 2); //See above for why this is 2.

      oldCommand.GetComponent<CommandDetails> ().scriptPosition = 0;
      oldCommand.GetComponent<CommandDetails> ().commandIndentation = 0.0f;
      oldCommand.transform.SetParent(commandsPane.transform, true);

      UpdatePositions ();
      UpdateScriptUI ();
   }

   //Called by children when the state of the command has changed. For instance, when a new command is added to a 'block' command.
   //Only the visuals need
   public void UpdateCommand(GameObject command) {
      UpdatePositions ();
      UpdateScriptUI ();
   }

   //Modifies the ordering position of each command in the list to account for any changes.
   private void UpdatePositions() {
      int position = 2; //Start at 2, as above
      foreach(GameObject command in commands) {
         command.GetComponent<CommandDetails> ().scriptPosition = position;
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
      float blockHeight = gameObject.GetComponent<RectTransform> ().sizeDelta.y / 2.0f; //Height of a single block.

      //Position commands.
      foreach(GameObject command in commands) {
         float indentation = command.GetComponent<CommandDetails> ().commandIndentation;
         command.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (indentation, -blockHeight * blocks - blockHeight / 2.0f);

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
         bodyPiece.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0.0f, -blockHeight * i - blockHeight / 2.0f);
      }

      //Move foot to bottom of script.
      foot.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0.0f, -blockHeight * Mathf.Max(blocks, 2));
   }

   public string collateScript() {
      scriptBody = ""; //Reset stored script, if any.

      foreach (GameObject command in commands) {
         scriptBody += command.GetComponent<CommandDetails> ().scriptCode.GetCode ();
      }

      return scriptHeader + scriptBody + scriptFooter;
   }

   public string NewLoopIdentifier() {
      loopIndexIdentifier += "i";
      return loopIndexIdentifier;
   }
}
