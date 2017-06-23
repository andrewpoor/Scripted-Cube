using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionSpawner : MonoBehaviour {
   //For spawning new conditions. Some things are needed to populate fields in the new command.
   public Transform conditionPrefab;
   public Canvas canvas;
   public Image commandsPane;

   public void SpawnNewCondition() {
      GameObject newCondition = Instantiate (conditionPrefab, Vector3.zero, transform.rotation).gameObject;
      newCondition.transform.SetParent (transform, false); //Apply scaling and position above spawner.

      newCondition.GetComponent<ConditionDragController> ().conditionSpawner = this;
      newCondition.GetComponent<ConditionDragController> ().canvas = canvas;
      newCondition.GetComponent<ConditionDragController> ().commandsPane = commandsPane;
   }
}
