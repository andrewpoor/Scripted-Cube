using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightEvent : MonoBehaviour {

   public delegate void AddRightCommand(int duration);
   public AddRightCommand command;

   public void AddCommand() {
      int duration;
      if (command != null && int.TryParse(GetComponent<InputField>().text, out duration)) {
         command (duration);
      }
   }
}
