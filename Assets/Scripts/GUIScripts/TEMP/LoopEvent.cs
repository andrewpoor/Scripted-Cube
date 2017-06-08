using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopEvent : MonoBehaviour {

   public delegate void AddLoopCommand(int duration);
   public AddLoopCommand command;

   public void AddCommand() {
      int numLoops;
      if (command != null && int.TryParse(GetComponent<InputField>().text, out numLoops)) {
         command (numLoops);
      }
   }
}
