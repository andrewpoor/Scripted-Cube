using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForwardEvent : MonoBehaviour {

   public delegate void AddForwardsCommand(int duration);
   public AddForwardsCommand command;

   public void AddCommand() {
      int duration;
      if (command != null && int.TryParse(GetComponent<InputField>().text, out duration)) {
         command (duration);
      }
   }
}
