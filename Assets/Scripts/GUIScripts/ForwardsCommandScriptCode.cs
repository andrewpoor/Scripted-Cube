using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForwardsCommandScriptCode : CommandScriptCode {

   public InputField stepsInput;

   public override string GetCode() {
      int numSteps;

      if(int.TryParse (stepsInput.text, out numSteps)) {
         return @"
      distance = Math.Min (" + numSteps + @", (int) Mathf.Round(parent.frontSensorHit.distance));
      timer = tileTime * distance;
      startPosition = parent.transform.position;
      targetPosition = startPosition + parent.transform.forward.normalized * distance;

      while(timer > 0f) {
         timer -= Time.deltaTime;
         parent.transform.position = Vector3.Lerp(startPosition, targetPosition, (tileTime * distance - Math.Max(timer, 0)) / (tileTime * distance));
         
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
