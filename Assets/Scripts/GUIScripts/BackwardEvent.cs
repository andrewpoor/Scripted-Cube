using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackwardEvent : MonoBehaviour {

   public delegate void AddBackwardsCommand(float duration);
   public AddBackwardsCommand command;

   public void AddCommand() {
      float duration;
      if (command != null && float.TryParse(GetComponent<InputField>().text, out duration)) {
         command (duration);
      }
   }
}
