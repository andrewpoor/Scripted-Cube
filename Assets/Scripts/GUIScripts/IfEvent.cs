using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IfEvent : MonoBehaviour {

   public delegate void AddIfCommand(string sensorType, string comparator, string value);
   public AddIfCommand command;

   public Dropdown sensorType;
   public Dropdown comparator;

   public void AddCommand() {
      if (command != null) {
         command (sensorType.captionText.text, comparator.captionText.text, GetComponent<InputField>().text);
      }
   }
}
