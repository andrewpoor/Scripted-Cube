using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootCommandScriptController : MonoBehaviour, IScriptController {
   
   public GameObject parentCommand; //The command above the block. Shared with the header.

   //Shouldn't ever be called.
   public void InsertIntoScript(List<GameObject> surroundingCommands) {
      Debug.Log ("Warning: Trying to insert a footer into the script!");
   }

   //Called by the child to propogate the command up to the nearest block.
   public void AddCommand(GameObject command, int positionOfAbove) {
      parentCommand.GetComponent<IScriptController>().AddCommand(command, positionOfAbove);
   }

   //Shouldn't ever be called.
   public void RemoveFromScript() {
      Debug.Log ("Warning: Trying to remove a footer from the script!");
   }

   //Called by the child to propogate the command up to the nearest block.
   public void RemoveCommand(GameObject command, int positionOfThis) {
      parentCommand.GetComponent<IScriptController>().RemoveCommand(command, positionOfThis);
   }

   public void UpdateParentCommand(GameObject newParentCommand) {
      parentCommand = newParentCommand;
   }

   public void UpdateChildsParentCommand(GameObject child) {
      if (child != null) {
         child.GetComponent<IScriptController> ().UpdateParentCommand (gameObject);
      } else {
         Debug.Log ("Updating parent of null child.");
      }
   }
      
   //Called by the child to propogate the command up to the nearest block.
   public void UpdateVisuals() {
      parentCommand.GetComponent<IScriptController> ().UpdateVisuals ();
   }
    
   //Shouldn't ever be called.
   public string CollateScript() {
      Debug.Log ("Warning: Trying to get the code of a footer!");

      return "";
   }

   public void SetScriptPosition(int position) {
      GetComponent<CommandDetails>().scriptPosition = position;
   }

   //Shouldn't be called.
   public void LightUp() {
      Debug.Log ("Warning: Trying to light up a non-action!");
   }

   //Shouldn't be called.
   public void Unlight() {
      Debug.Log ("Warning: Trying to unlight a non-action!");
   }
}
