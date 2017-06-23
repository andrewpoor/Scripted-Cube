using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightCommandScriptCode : CommandScriptCode {

   public InputField turnsInput;

   public override string GetCode() {
      GlobalScriptController globalScriptController = GetComponent<CommandDragController> ().commandSpawner.globalScriptController;
      int actionID = globalScriptController.RegisterAction (gameObject);

      int numTurns;

      if(int.TryParse (turnsInput.text, out numTurns)) {
         return @"
      rotations = " + numTurns + @";
      actionID = " + actionID + @";
      startRotation = parent.transform.rotation;
      targetRotation = startRotation * Quaternion.Euler(Vector3.up * 90 * rotations);
      timer = 0.0f;
      totalTime = rotations;

      parent.LightUpAction(actionID);

      while(parent.transform.rotation != targetRotation) {
         timer += Time.deltaTime / parent.spinTime;
         parent.transform.rotation = startRotation * Quaternion.Euler(Vector3.up * Mathf.Lerp(0.0f, 90 * rotations, 1 - (totalTime - timer) / totalTime));
         
         yielded = true;
         yield return null;
      }

      parent.UnlightAction(actionID);
      ";
      } else {
         //Error?
         return "";
      }
   }
}