using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackwardEvent : MonoBehaviour {

   public delegate void AddBackwardsCommand(int duration);
   public AddBackwardsCommand command;

   public void AddCommand() {
      int duration;
      if (command != null && int.TryParse(GetComponent<InputField>().text, out duration)) {
         command (duration);
      }
   }
}
