using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

   public GameObject player;

   private Vector3 offset;

   void Start ()
   {
      transform.position = transform.position + player.transform.position;
      offset = transform.position - player.transform.position;
   }

   //Happens once per frame, after all other operations
   void LateUpdate ()
   {
      transform.position = player.transform.position + offset;
   }
}