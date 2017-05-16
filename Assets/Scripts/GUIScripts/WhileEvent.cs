using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhileEvent : MonoBehaviour {

   public delegate void AddWhileCommand(string sensorType, string comparator, int value);
   public AddWhileCommand command;

   public Dropdown sensorType;
   public Dropdown comparator;

   public void AddCommand() {
      int value;
      if (command != null && int.TryParse(GetComponent<InputField>().text, out value)) {
         command (sensorType.captionText.text, comparator.captionText.text, value);
      }
   }
}
