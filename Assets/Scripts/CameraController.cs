using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

   public GameObject player;
   public float speed; //Speed camera moves when manually moving it.

   private Vector3 playerOffset; //Offset to player.
   private Vector3 userAddedOffset; //Additional offset when the user moves the camera manually.

   void Start ()
   {
      transform.position = transform.position + player.transform.position;
      playerOffset = transform.position - player.transform.position;
      userAddedOffset = Vector3.zero;
   }

   void Update()
   {
      float horizontal = Input.GetAxisRaw ("Horizontal");
      float vertical = Input.GetAxisRaw ("Vertical");

      Vector3 movement = new Vector3(horizontal, 0f, vertical);
      movement = movement.normalized * speed * Time.deltaTime;

      userAddedOffset += movement;
   }

   //Happens once per frame, after all other operations
   void LateUpdate ()
   {
      transform.position = player.transform.position + playerOffset + userAddedOffset;
   }
}