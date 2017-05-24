using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

   public float timeToDisappear = 1.0f; //Time taken to fade away when collected.

   private MeshRenderer[] meshRenderers; //Renderers of children objects.
   private float timer = 0.0f;
   private bool disappear = false; //If true, begin the disappearing sequence.

   void Start () {
      meshRenderers = GetComponentsInChildren<MeshRenderer> ();
   }

   void Update() {
      if(disappear) {
         foreach(MeshRenderer meshRenderer in meshRenderers) {
            Color color = meshRenderer.material.color;
            color.a -= (1.0f / timeToDisappear) * Time.deltaTime;
            meshRenderer.material.color = color;
         }

         timer += Time.deltaTime;

         if (timer > timeToDisappear) {
            gameObject.SetActive (false);
         }
      }
   }

   //Unlocks the door to allow the player through.
   public void Unlock() {
      disappear = true;
   }
}
