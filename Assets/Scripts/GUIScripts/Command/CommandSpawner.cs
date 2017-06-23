using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandSpawner : MonoBehaviour {
   //For spawning new commands. Some things are needed to populate fields in the new command.
   public Transform commandPrefab;
   public Canvas canvas;
   public Image commandsPane;
   public GlobalScriptController globalScriptController; //For loops.

   public void SpawnNewCommand() {
      GameObject newCommand = Instantiate (commandPrefab, Vector3.zero, transform.rotation).gameObject;
      newCommand.transform.SetParent (transform, false); //Apply scaling and position above spawner.

      newCommand.GetComponent<CommandDragController> ().commandSpawner = this;
      newCommand.GetComponent<CommandDragController> ().canvas = canvas;
      newCommand.GetComponent<CommandDragController> ().commandsPane = commandsPane;
   }
}
