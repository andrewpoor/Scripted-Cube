using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightEvent : MonoBehaviour {

   public delegate void AddRightCommand(float duration);
   public AddRightCommand command;

   public void AddCommand() {
      float duration;
      if (command != null && float.TryParse(GetComponent<InputField>().text, out duration)) {
         command (duration);
      }
   }
}
