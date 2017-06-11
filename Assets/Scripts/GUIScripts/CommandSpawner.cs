using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandSpawner : MonoBehaviour {

   //For spawning new commands. Some things are needed to populate fields in the new command.
   public Transform commandPrefab;
   public GlobalScriptController scriptController;
   public Canvas canvas;

   private RectTransform rTransform;
   private Vector2 canvasScale; //Used to account for different resolutions.

   public void Start() {
      rTransform = GetComponent<RectTransform> ();
      canvasScale = new Vector2 (Screen.width / canvas.GetComponent<CanvasScaler> ().referenceResolution.x, Screen.height / canvas.GetComponent<CanvasScaler> ().referenceResolution.y);
   }

   public void SpawnNewCommand() {
      GameObject newCommand = Instantiate (commandPrefab, Vector3.zero, transform.rotation).gameObject;
      newCommand.transform.SetParent (transform, false); //Apply scaling and position above spawner.

      newCommand.GetComponent<CommandDragController> ().commandSpawner = this;
      newCommand.GetComponent<CommandDragController> ().canvas = canvas;
      newCommand.GetComponent<CommandScriptController> ().globalScriptController = scriptController;
   }
}
