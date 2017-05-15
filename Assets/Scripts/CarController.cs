using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Holds information on an individual axle
[System.Serializable]
public class AxleInfo {
   public WheelCollider leftWheel;
   public WheelCollider rightWheel;
   public bool motor;
   public bool steering;
}

public class CarController : MonoBehaviour {
   public List<AxleInfo> axleInfos;
   public Vector3 centerOfMass;

   void Start() {
      GetComponent<Rigidbody> ().centerOfMass = centerOfMass;
   }

   // finds the corresponding visual wheel
   // correctly applies the transform
   public void ApplyLocalPositionToVisuals(WheelCollider collider)
   {
      if (collider.transform.childCount == 0) {
         return;
      }

      Transform visualWheel = collider.transform.GetChild(0);

      Vector3 position;
      Quaternion rotation;
      collider.GetWorldPose(out position, out rotation);

      //Account for off-rotation of visual.
      Vector3 eulerRotation = rotation.eulerAngles;
      eulerRotation.z += 90;
      rotation.eulerAngles = eulerRotation;

      visualWheel.transform.position = position;
      visualWheel.transform.rotation = rotation;
   }
}