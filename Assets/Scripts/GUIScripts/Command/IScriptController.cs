/*
 * All command scripts extend this interface to provide unified behaviour.
 * Adding and removing commands is recursive, so this allows for different command scripts to be treated the same.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScriptController {
   void InsertIntoScript(List<GameObject> surroundingCommands); //Called by drag controller to insert into the topmost surrounding command.
   void AddCommand(GameObject command, int positionOfAbove); //Called by children, to be passed up recursively.
   void RemoveFromScript(); //Called by drag controller to remove from the overall script.
   void RemoveCommand(GameObject command, int positionOfThis); //Called by children, to be passed up recursively.
   void UpdateParentCommand(GameObject newParentCommand); //Called by nearest block, to set parent of this command.
   void UpdateChildsParentCommand(GameObject child); //Called by nearest block, to set the parent of the given child to be this command.
   void UpdateVisuals(); //Called by children, to be passed up recursively.
   string CollateScript(); //Called by nearest block, to collate the entire script at the highest level.
   void SetScriptPosition(int position); //Called by nearest block, to set the position of all elements in the command appropriately.
   void LightUp();
   void Unlight();
}
