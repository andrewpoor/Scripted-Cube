/*
 * Represents the command at an abstract level.
 * 
 * Handles abstract positioning of the command within the chain of commands forming the script.
 * Also stores the code that is written by this block.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandScriptController : MonoBehaviour {

   public GlobalScriptController globalScriptController; //Reference to the global script controller that manages the script as a whole.
   public CommandDetails commandDetails; //Stores simple data about this command.

   //Put this command into the global script below the topmost command touching this one.
   //The surrounding commands are all the commands this one is touching in the UI.
   public void InsertIntoScript(List<GameObject> surroundingCommands) {
      int positionOfTopmost = 0; //0 indicates no surrounding command is in the main script.

      foreach(GameObject command in surroundingCommands) {
         int otherPosition = command.GetComponent<CommandDetails> ().scriptPosition;

         if ((positionOfTopmost == 0) || (otherPosition > 0 && otherPosition < positionOfTopmost)) {
            positionOfTopmost = otherPosition;
         }
      }

      if (positionOfTopmost > 0) {
         globalScriptController.AddCommand (gameObject, positionOfTopmost);
      }
   }

   //Take this command out of the global script, such that any commands below it take the place of this one.
   public void RemoveFromScript() {
      if (commandDetails.scriptPosition > 0) {
         globalScriptController.RemoveCommand (gameObject, commandDetails.scriptPosition);
      }
   }
}
