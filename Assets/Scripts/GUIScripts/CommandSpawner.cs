using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSpawner : MonoBehaviour {

   public GameObject CommandsPane; //The temporary parent of all spawned commands.

   //For spawning new commands. Some things are needed to populate fields in the new command.
   public Transform commandPrefab;
   public GlobalScriptController scriptController;

   public void SpawnNewCommand() {
      GameObject newCommand = Instantiate (commandPrefab, transform.position, transform.rotation).gameObject;
      newCommand.transform.SetParent (CommandsPane.transform, true);

      newCommand.GetComponent<CommandDragController> ().commandSpawner = this;
      newCommand.GetComponent<CommandScriptController> ().globalScriptController = scriptController;
   }
}
