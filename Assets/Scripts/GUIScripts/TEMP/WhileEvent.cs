using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhileEvent : MonoBehaviour {

   public delegate void AddWhileCommand(string sensorType, string comparator, string value);
   public AddWhileCommand command;

   public Dropdown sensorType;
   public Dropdown comparator;

   public void AddCommand() {
      if (command != null) {
         command (sensorType.captionText.text, comparator.captionText.text, GetComponent<InputField>().text);
      }
   }
}
