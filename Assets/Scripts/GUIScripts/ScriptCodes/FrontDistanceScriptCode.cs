using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrontDistanceScriptCode : MonoBehaviour , IConditionScriptCode {

   public Dropdown equalityInput; //Displays "is" or "isn't", for == or !=.
   public InputField squaresInput; //To input the number of squares desired.

   public string GetCondition() {
      int numSquares;

      if(int.TryParse (squaresInput.text, out numSquares)) {
         string operation = "";
         if (equalityInput.captionText.text == "≠") {
            operation = "!=";
         } else if (equalityInput.captionText.text == "≤") {
            operation = "<=";
         } else if (equalityInput.captionText.text == "≥") {
            operation = ">=";
         } else if (equalityInput.captionText.text == "=") {
            operation = "==";
         } else { //If "<" or ">" is the input text
            operation = equalityInput.captionText.text;
         }

         return "Mathf.Round(parent.frontSensorHit.distance) " + operation + " " + numSquares;
      } else {
         //Error?
         return "";
      }
   }
}
