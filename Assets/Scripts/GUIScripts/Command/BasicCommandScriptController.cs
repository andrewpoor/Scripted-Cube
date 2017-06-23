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

public class BasicCommandScriptController : MonoBehaviour, IScriptController {
   
   public CommandDetails commandDetails; //Stores simple data about this command.
   public GameObject parentCommand; //The command above this one.
   public Image connector; //The visual connector to join with other pieces.

   public float brightnessFactor = 0.3f; //Between 0 and 1, controls how close to white the command gets when lit up.

   private Color baseColour;

   void Start() {
      baseColour = GetComponent<Image> ().color;
   }

   //Put this command into the global script below the topmost command touching this one.
   //The surrounding commands are all the commands this one is touching in the UI.
   public void InsertIntoScript(List<GameObject> surroundingCommands) {
      int positionOfTopmost = 0; //0 indicates no surrounding command is in the main script.

      foreach(GameObject command in surroundingCommands) {
         int otherPosition = command.GetComponent<CommandDetails> ().scriptPosition;

         if ((positionOfTopmost == 0) || (otherPosition > 0 && otherPosition < positionOfTopmost)) {
            positionOfTopmost = otherPosition;
            parentCommand = command;
         }
      }

      if (positionOfTopmost > 0) {
         parentCommand.GetComponent<IScriptController> ().AddCommand (gameObject, positionOfTopmost);
      }
   }

   //Called by the child to propogate the command up to the nearest block.
   public void AddCommand(GameObject command, int positionOfAbove) {
      parentCommand.GetComponent<IScriptController>().AddCommand(command, positionOfAbove);
   }

   //Called by the child to propogate the command up to the nearest block.
   public void RemoveCommand(GameObject command, int position) {
      parentCommand.GetComponent<IScriptController>().RemoveCommand(command, position);
   }

   //Take this command out of the global script, such that any commands below it take the place of this one.
   public void RemoveFromScript() {
      if (commandDetails.scriptPosition > 0) {
         parentCommand.GetComponent<IScriptController>().RemoveCommand(gameObject, commandDetails.scriptPosition);
      }

      parentCommand = null;
   }

   //Called by the nearest block to update the parenting within it.
   public void UpdateParentCommand(GameObject newParentCommand) {
      parentCommand = newParentCommand;
   }

   public void UpdateChildsParentCommand(GameObject child) {
      if (child != null) {
         child.GetComponent<IScriptController> ().UpdateParentCommand (gameObject);
      } else {
         Debug.Log ("Warning: Updating parent of null child!");
      }
   }

   //Called by the child to propogate the command up to the nearest block.
   public void UpdateVisuals() {
      parentCommand.GetComponent<IScriptController> ().UpdateVisuals ();
   }

   //Get the script for this command.
   public string CollateScript() {
      return commandDetails.scriptCode.GetCode ();
   }

   public void SetScriptPosition(int position) {
      commandDetails.scriptPosition = position;
   }

   //Light up the command to indicate that it's currently being run.
   public void LightUp() {
      GetComponent<Image> ().color = Color.Lerp (baseColour, Color.white, brightnessFactor);
      connector.color = GetComponent<Image> ().color;
   }

   //Unlight the command to indicate it is not being run.
   public void Unlight() {
      GetComponent<Image> ().color = baseColour;
      connector.color = GetComponent<Image> ().color;
   }
}
