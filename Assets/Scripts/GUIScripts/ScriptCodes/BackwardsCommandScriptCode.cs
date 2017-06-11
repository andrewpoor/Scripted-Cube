using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackwardsCommandScriptCode : CommandScriptCode {

   public InputField stepsInput;

   public override string GetCode() {
      int numSteps;

      if(int.TryParse (stepsInput.text, out numSteps)) {
         return @"
      distance = Math.Min (" + numSteps + @", (int) Mathf.Round(parent.rearSensorHit.distance));
      startPosition = parent.transform.position;
      targetPosition = startPosition - parent.transform.forward.normalized * distance;
      timer = 0.0f;
      totalTime = distance;

      while(parent.transform.position != targetPosition) {
         timer += Time.deltaTime / parent.tileTime;
         parent.transform.position = Vector3.Lerp(startPosition, targetPosition,  1 - (totalTime - timer) / totalTime);
         
         yielded = true;
         yield return null;
      }
      ";
      } else {
         //Error?
         return "";
      }
   }
}