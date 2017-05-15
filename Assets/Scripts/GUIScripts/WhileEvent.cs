using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhileEvent : MonoBehaviour {

   public delegate void AddWhileCommand(string sensorType, string comparator, float value);
   public AddWhileCommand command;

   public Dropdown sensorType;
   public Dropdown comparator;

   public void AddCommand() {
      float value;
      if (command != null && float.TryParse(GetComponent<InputField>().text, out value)) {
         command (sensorType.captionText.text, comparator.captionText.text, value);
      }
   }
}
