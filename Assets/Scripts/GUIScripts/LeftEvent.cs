using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftEvent : MonoBehaviour {

   public delegate void AddLeftCommand(float duration);
   public AddLeftCommand command;

   public void AddCommand() {
      float duration;
      if (command != null && float.TryParse(GetComponent<InputField>().text, out duration)) {
         command (duration);
      }
   }
}
