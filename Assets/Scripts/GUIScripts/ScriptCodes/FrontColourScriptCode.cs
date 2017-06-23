using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrontColourScriptCode : MonoBehaviour, IConditionScriptCode {

   public Dropdown equalityInput; //Displays "is" or "isn't", for == or !=.
   public Dropdown colourInput; //Displays the list of possible colours.

   public string GetCondition() {
      string negation = "";
      if (equalityInput.captionText.text == "isn't") {
         negation = "!";
      }

      return negation + "parent.frontSensorHit.collider.gameObject.CompareTag(\"Colour" + colourInput.captionText.text + "\")";
   }
}
