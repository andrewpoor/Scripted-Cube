using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForwardEvent : MonoBehaviour {

   public delegate void AddForwardsCommand(float duration);
   public AddForwardsCommand command;

   public void AddCommand() {
      float duration;
      if (command != null && float.TryParse(GetComponent<InputField>().text, out duration)) {
         command (duration);
      }
   }
}
