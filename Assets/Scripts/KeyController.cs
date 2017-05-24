using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour {

   public float timeToDisappear = 1.0f; //Time taken to fade away when collected.
   public float scalingRate = 1.0f; //Rate the object grows, additive to the current scale per second.

   private List<GameObject> linkedDoors;
   private MeshRenderer meshRenderer;
   private Vector3 scaling;
   private float timer = 0;
   private bool disappear = false; //If true, begin the disappearing sequence.

   void Start () {
      string colourTag = transform.Find ("KeyIdentifier").gameObject.tag;
      GameObject[] allDoors = GameObject.FindGameObjectsWithTag ("Door");
      linkedDoors = new List<GameObject> ();
      foreach(GameObject door in allDoors) {
         GameObject doorIdentifier = door.transform.Find ("DoorIdentifier").gameObject;
         if(doorIdentifier.CompareTag(colourTag)) {
            linkedDoors.Add(door);
         }
      }

      meshRenderer = GetComponent<MeshRenderer> ();
      scaling = new Vector3 (scalingRate, scalingRate, scalingRate);
   }

   void Update () {
      if(disappear) {
         transform.localScale += scaling * Time.deltaTime;

         Color color = meshRenderer.material.color;
         color.a -= (1.0f / timeToDisappear) * Time.deltaTime;
         meshRenderer.material.color = color;

         timer += Time.deltaTime;

         if (timer > timeToDisappear) {
            gameObject.SetActive (false);
         }
      }
   }

   void OnTriggerEnter(Collider other) {
      if(other.gameObject.CompareTag("Player") && !disappear) {
         disappear = true;

         foreach(GameObject door in linkedDoors) {
            door.BroadcastMessage ("Unlock");
         }
      }
   }
}
