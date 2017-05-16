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

   public float wallSensorRange = 100f;
   public RaycastHit frontSensorHit; //Details front sensor detects.
   public bool frontSensorDetected; //True if the front sensor has detected something.
   public RaycastHit rearSensorHit; //Details rear sensor detects.
   public bool rearSensorDetected; //True if the rear sensor has detected something.
   public float tileTime = 1.0f; //Time taken for the player to move one tile.
   public float spinTime = 1.0f; //Time taken for the player to rotate 90 degrees.

   private bool run; //If true, run the custom script.
   private string scriptRepresentation; //A visual representation of the script written so far.
   private int indentationLevel = 0; //Indentation level of the script representation.

   private Ray frontSensor = new Ray(); //Detects walls and other objects ahead of player.
   private Ray rearSensor = new Ray(); //Detects walls and other objects behind the player.
   private int horizontalSensorDetectable; //Layer for things detectable with the front sensor.

   //For dynamically loading the user's script.
   private System.Reflection.Assembly movementAssembly;
   private string scriptPath;
   private IEnumerator scriptCoroutine;

   //Boilerplate code for assembling the user's script.
   private string scriptHeader;
   private string scriptBody;
   private string scriptFooter;

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
      scriptHeader = @"
using UnityEngine;
using System.Collections;
using System;

public class ScriptedPlayerController : DynamicPlayerController
{
   public IEnumerator GetCoroutine(PlayerController parent) {
      return CustomScript(parent);
   }

   public IEnumerator CustomScript(PlayerController parent) {
   float timer = 0f;
   int distance = 0;
   int rotations = 0;
   float tileTime = " + tileTime + @"f;
   float spinTime = " + spinTime + @"f;
   Vector3 targetPosition;
   Quaternion startRotation;
   Quaternion targetRotation;
   ";

      scriptBody = "";

      scriptFooter = @"
   }
}
   ";

      winLoseMessage.text = "";
      run = false;
      scriptRepresentation = "";
      horizontalSensorDetectable = LayerMask.GetMask ("HorizontalSensorDetectable");
      frontSensorDetected = false;
      rearSensorDetected = false;
   }

   // Update is called once per frame
   void Update () {
      frontSensor.origin = transform.position + 0.5f * transform.forward.normalized;
      frontSensor.direction = transform.forward;
      frontSensorDetected = Physics.Raycast (frontSensor, out frontSensorHit, wallSensorRange, horizontalSensorDetectable);
      Debug.DrawRay (frontSensor.origin, frontSensor.direction * wallSensorRange, Color.red);

      if (frontSensorDetected) {
         scriptPreview.text = "Hit: " + frontSensorHit.distance + "m. Script: " + scriptRepresentation;
      } else {
         scriptPreview.text = "Nope. Script: " + scriptRepresentation;
      }

      rearSensor.origin = transform.position;
      rearSensor.direction = -transform.forward;
      rearSensorDetected = Physics.Raycast (rearSensor, out rearSensorHit, wallSensorRange, horizontalSensorDetectable);
   }

   // FixedUpdate is called once per frame, before physics calculations
   void FixedUpdate() {
      //If it's time to run the script, do so.
      if (run) {
         Debug.Log ("Running");

         run = false; //Only run one instance of the script at a time.

         IEnumerator script = BeginScript ();
         StartCoroutine(script);
      }
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

   //The following functions are controlled by the UI elements to modify and run the custom script.

   public void AddForwardStep(int duration)
   {
      scriptBody += @"
      distance = Math.Min (" + duration + @", (int) parent.frontSensorHit.distance);
      ";

      scriptBody += @"
      timer = tileTime * distance;
      targetPosition = parent.transform.position + parent.transform.forward.normalized * distance;

      while(timer > 0f) {
         timer -= Time.deltaTime;
         parent.transform.position += parent.transform.forward.normalized * Math.Min(Time.deltaTime, timer) / tileTime;

         yield return null;
      }

      parent.transform.position = targetPosition;

      yield return null;
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Move " + duration.ToString() + " squares forwards.";
   }

   public void AddBackwardStep(int duration)
   {
      scriptBody += @"
      distance = Math.Min (" + duration + @", (int) parent.rearSensorHit.distance);
      ";

      scriptBody += @"
      timer = tileTime * distance;
      targetPosition = parent.transform.position - parent.transform.forward.normalized * distance;

      while(timer > 0f) {
         timer -= Time.deltaTime;
         parent.transform.position -= parent.transform.forward.normalized * Math.Min(Time.deltaTime, timer) / tileTime;

         yield return null;
      }

      parent.transform.position = targetPosition;

      yield return null;
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Move " + duration.ToString() + " squares backwards.";
   }

   public void AddRightwardStep(int duration)
   {
      scriptBody += @"
      rotations = " + duration + @";
      ";

      scriptBody += @"
      timer = spinTime * rotations;
      startRotation = parent.transform.rotation;
      targetRotation = Quaternion.LookRotation(new Vector3 ((startRotation.eulerAngles.x + rotations * 90) % 360, startRotation.eulerAngles.y, startRotation.eulerAngles.z), Vector3.up);
      
      while(timer > 0f) {
         timer -= Time.deltaTime;
         parent.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, (spinTime * rotations - Math.Max(timer, 0)) / (spinTime * rotations));
         yield return null;
      }

      parent.transform.rotation = targetRotation;

      yield return null;
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Turn right " + (duration * 90).ToString() + " degrees.";
   }

   public void AddLeftwardStep(float duration)
   {
      scriptBody += @"
      rotations = " + duration + @";
      ";

      scriptBody += @"
      timer = spinTime * rotations;
      startRotation = parent.transform.rotation;
      targetRotation = Quaternion.LookRotation(new Vector3 ((startRotation.eulerAngles.x - rotations * 90) % 360, startRotation.eulerAngles.y, startRotation.eulerAngles.z), Vector3.up);
      
      while(timer > 0f) {
         timer -= Time.deltaTime;
         parent.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, (spinTime * rotations - Math.Max(timer, 0)) / (spinTime * rotations));
         yield return null;
      }

      parent.transform.rotation = targetRotation;

      yield return null;
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Turn left " + (duration * 90).ToString() + " degrees.";
   }

   public void StartWhile(string sensorType, string op, int value) {
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
    
   //Reset all of the game's components
   public void ResetScript()
   {
      SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
   }

   //Compile and run the script generated by the user
   public void RunScript()
   {
      if(scriptBody != "") {
         string script = scriptHeader + scriptBody + scriptFooter;

         //Compile the script and extract its coroutine, which contains the actions for the player to perform.
         movementAssembly = RuntimeCompiler.Compile(script);
         DynamicPlayerController dpc = (DynamicPlayerController) movementAssembly.CreateInstance("ScriptedPlayerController");
         scriptCoroutine = dpc.GetCoroutine (this);

         run = true;
      }
   }
}
