using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//The dynamically loaded scripts must inherit from this, to provide a public API.
public interface DynamicPlayerController {
   IEnumerator GetCoroutine(PlayerController parent); //Returns the coroutine that holds the scripted actions.
}

public class PlayerController : MonoBehaviour {
   public Text winLoseMessage;
   public Text scriptPreview;

   public ForwardEvent forwardEvent;
   public BackwardEvent backwardEvent;
   public RightEvent rightEvent;
   public LeftEvent leftEvent;
   public WhileEvent whileEvent;

   public float frontSensorRange = 100f;
   public RaycastHit frontSensorHit; //Details sensor detects.
   public bool frontSensorDetected; //True if the sensor has detected something.

   private bool run; //If true, run the custom script.
   private Vector3 startPosition;
   private Quaternion startRotation;
   private string scriptRepresentation; //A visual representation of the script written so far.
   private int indentationLevel = 0; //Indentation level of the script representation.
   private CarController carController; //Reference to the 'car' part of the robot.

   private Ray frontSensor; //Detects walls and other objects ahead of player.
   private int frontSensorDetectable; //Layer for things detectable with the front sensor.

   //For dynamically loading the user's script.
   private System.Reflection.Assembly movementAssembly;
   private string scriptPath;
   private IEnumerator scriptCoroutine;

   //Boilerplate code for assembling the user's script.
   private string scriptHeader = @"
using UnityEngine;
using System.Collections;

public class ScriptedPlayerController : DynamicPlayerController
{
   public IEnumerator GetCoroutine(PlayerController parent) {
      return CustomScript(parent);
   }

   public IEnumerator CustomScript(PlayerController parent) {
      float duration = 1f;
      float timer = 0f;
      CarController carController = parent.GetCarController();

   ";

   private string scriptBody = "";

   private string scriptFooter = @"
   }
}
   ";

   void OnEnable() {
      forwardEvent.command +=  AddForwardStep;
      backwardEvent.command += AddBackwardStep;
      rightEvent.command +=    AddRightwardStep;
      leftEvent.command +=     AddLeftwardStep;
      whileEvent.command +=    StartWhile;
   }

   void OnDisable() {
      forwardEvent.command -=  AddForwardStep;
      backwardEvent.command -= AddBackwardStep;
      rightEvent.command -=    AddRightwardStep;
      leftEvent.command -=     AddLeftwardStep;
      whileEvent.command -=    StartWhile;
   }

   // Use this for initialization
   void Start () {
      winLoseMessage.text = "";
      run = false;
      startPosition = transform.localPosition;
      startRotation = transform.localRotation;
      scriptRepresentation = "";
      carController = GetComponentInChildren<CarController> ();
      frontSensorDetectable = LayerMask.GetMask ("FrontSensorDetectable");
   }

   // Update is called once per frame
   void Update () {
      frontSensor.origin = transform.position - transform.forward;
      frontSensor.direction = -transform.forward;
      frontSensorDetected = Physics.Raycast (frontSensor, out frontSensorHit, frontSensorRange, frontSensorDetectable);
      Debug.DrawRay (frontSensor.origin, frontSensor.direction * frontSensorRange, Color.red);

      if (frontSensorDetected) {
         scriptPreview.text = "Hit: " + frontSensorHit.distance + "m. Script: " + scriptRepresentation;
      } else {
         scriptPreview.text = "Nope. Script: " + scriptRepresentation;
      }
   }

   // FixedUpdate is called once per frame, before physics calculations
   void FixedUpdate() {
      //Once only, start the user's script.
      //The script will continue every frame until completion.
      if (run) {
         run = false; //Only run one instance of the script at a time.

         IEnumerator script = BeginScript ();
         StartCoroutine(script);
      }
   }

   public CarController GetCarController() {
      return carController;
   }

   IEnumerator BeginScript()
   {
      //Wait for a moment first, to avoid visual frame issues.
      yield return new WaitForSeconds (0.5f);

      //Run the user's script.
      StartCoroutine(scriptCoroutine);
      yield return null;
   }

   void OnTriggerEnter( Collider other )
   {
      if( other.gameObject.CompareTag("Succeed") )
      {
         winLoseMessage.text = "You win!";
      } else if (other.gameObject.CompareTag("Fail") )
      {
         winLoseMessage.text = "Game over";
      }
   }

   private void ResetPlayer()
   {
      transform.localPosition = startPosition;
      transform.localRotation = startRotation;
   }

   //The following functions are controlled by the UI elements to modify and run the custom script.

   public void AddForwardStep(float duration)
   {
      scriptBody += @"
      duration = " + duration.ToString() + @"f;
      ";

      scriptBody += @"
      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 0f;
            axleInfo.rightWheel.brakeTorque = 0f;
            axleInfo.leftWheel.motorTorque = -300f;
            axleInfo.rightWheel.motorTorque = -300f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      yield return new WaitForSeconds (duration);

      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 500f;
            axleInfo.rightWheel.brakeTorque = 500f;
            axleInfo.leftWheel.motorTorque = 0f;
            axleInfo.rightWheel.motorTorque = 0f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      duration = 1f;

      yield return new WaitForSeconds (3 * duration);

      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Move forwards for " + duration.ToString() + " seconds.";
   }

   public void AddBackwardStep(float duration)
   {
      scriptBody += @"
      duration = " + duration.ToString() + @"f;
      ";
      
      scriptBody += @"
      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 0f;
            axleInfo.rightWheel.brakeTorque = 0f;
            axleInfo.leftWheel.motorTorque = 300f;
            axleInfo.rightWheel.motorTorque = 300f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      yield return new WaitForSeconds (duration);

      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 500f;
            axleInfo.rightWheel.brakeTorque = 500f;
            axleInfo.leftWheel.motorTorque = 0f;
            axleInfo.rightWheel.motorTorque = 0f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      duration = 1f;

      yield return new WaitForSeconds (3 * duration);

      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Move backwards for " + duration.ToString() + " seconds.";
   }

   public void AddRightwardStep(float duration)
   {
      scriptBody += @"
      duration = " + duration.ToString() + @"f;
      ";

      scriptBody += @"
      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 0f;
            axleInfo.rightWheel.brakeTorque = 0f;
            axleInfo.leftWheel.motorTorque = -110f;
            axleInfo.rightWheel.motorTorque = 110f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      yield return new WaitForSeconds (duration);

      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 500f;
            axleInfo.rightWheel.brakeTorque = 500f;
            axleInfo.leftWheel.motorTorque = 0f;
            axleInfo.rightWheel.motorTorque = 0f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      duration = 1f;

      yield return new WaitForSeconds (3 * duration);

      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Turn right for " + duration.ToString() + " seconds.";
   }

   public void AddLeftwardStep(float duration)
   {
      scriptBody += @"
      duration = " + duration.ToString() + @"f;
      ";

      scriptBody += @"
      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 0f;
            axleInfo.rightWheel.brakeTorque = 0f;
            axleInfo.leftWheel.motorTorque = 110f;
            axleInfo.rightWheel.motorTorque = -110f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      yield return new WaitForSeconds (duration);

      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 500f;
            axleInfo.rightWheel.brakeTorque = 500f;
            axleInfo.leftWheel.motorTorque = 0f;
            axleInfo.rightWheel.motorTorque = 0f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      duration = 1f;

      yield return new WaitForSeconds (3 * duration);

      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Turn left for " + duration.ToString() + " seconds.";
   }

   public void StartWhile(string sensorType, string op, float value) {
      if(sensorType == "Front Sensor Distance") {
         scriptBody += @"
         while(parent.frontSensorHit.distance " + op + " " + value + @") {
         ";

         scriptRepresentation += "\n";
         for(int i = 0; i < indentationLevel; i++) {
            scriptRepresentation += "   ";
         }
         scriptRepresentation += "While(FrontSensorDistance " + op + " " + value + ") {";

         indentationLevel += 1;
      } else if(sensorType == "Front Sensor Colour") {
         scriptRepresentation += "\nSensor type not yet supported.";
      } else if(sensorType == "Ground Sensor Colour") {
         scriptRepresentation += "\nSensor type not yet supported.";
      } else {
         scriptRepresentation += "\nUnknown sensor type used.";
      }
   }

   public void EndWhile() {
      scriptBody += @"
      }
      ";

      indentationLevel = Math.Max(0, indentationLevel - 1);

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "}";
   }

   public void MoveForwards() {
      scriptBody += @"
      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 0f;
            axleInfo.rightWheel.brakeTorque = 0f;
            axleInfo.leftWheel.motorTorque = -300f;
            axleInfo.rightWheel.motorTorque = -300f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      yield return null;
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Move forwards.";
   }

   public void MoveBackwards() {
      scriptBody += @"
      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 0f;
            axleInfo.rightWheel.brakeTorque = 0f;
            axleInfo.leftWheel.motorTorque = 300f;
            axleInfo.rightWheel.motorTorque = 300f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      yield return null;
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Move backwards.";
   }

   public void TurnRight() {
      scriptBody += @"
      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 0f;
            axleInfo.rightWheel.brakeTorque = 0f;
            axleInfo.leftWheel.motorTorque = -300f;
            axleInfo.rightWheel.motorTorque = 300f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      yield return null;
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Turn right.";
   }

   public void TurnLeft() {
      scriptBody += @"
      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 0f;
            axleInfo.rightWheel.brakeTorque = 0f;
            axleInfo.leftWheel.motorTorque = 300f;
            axleInfo.rightWheel.motorTorque = -300f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      yield return null;
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Turn Left.";
   }

   public void Brake() {
      scriptBody += @"
      foreach (AxleInfo axleInfo in carController.axleInfos) {
         if (axleInfo.steering) {
            axleInfo.leftWheel.steerAngle = 0f;
            axleInfo.rightWheel.steerAngle = 0f;
         }
         if (axleInfo.motor) {
            axleInfo.leftWheel.brakeTorque = 500000f;
            axleInfo.rightWheel.brakeTorque = 500000f;
            axleInfo.leftWheel.motorTorque = 0f;
            axleInfo.rightWheel.motorTorque = 0f;
         }
         carController.ApplyLocalPositionToVisuals(axleInfo.leftWheel);
         carController.ApplyLocalPositionToVisuals(axleInfo.rightWheel);
      }

      yield return new WaitForSeconds (3 * duration);
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Brake.";
   }
    
   //Reset all of the game's components
   public void ResetScript()
   {
      SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
   }

   //Compile and run the script generated by the user
   public void RunScript()
   {
      if(scriptBody != "") {
         //      scriptPath = Application.dataPath + "/playerScript.txt";
         //      var scriptFile = File.CreateText(scriptPath);
         //      scriptFile.Write(script);
         //      scriptFile.Close();

         //      script = RuntimeCompiler.Load(scriptPath);
         //
         //      if (script == null)
         //      {
         //         throw new FileLoadException();
         //      }

         string script = scriptHeader + scriptBody + scriptFooter;

         //Compile the script and extract its coroutine, which contains the actions for the player to perform.
         movementAssembly = RuntimeCompiler.Compile(script);
         DynamicPlayerController dpc = (DynamicPlayerController) movementAssembly.CreateInstance("ScriptedPlayerController");
         scriptCoroutine = dpc.GetCoroutine (this);

         run = true;
      }
   }
}
