using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IColliderController {

   void ShrinkCollider (); //Make the collider smaller and position it at the top.
   void ExpandCollider(); //Make the collider full-sized and centred.
   void EnableCollider(bool enabled); //Enable or disable the collider appropriately.
}
