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
   public IfEvent ifEvent;
   public LoopEvent loopEvent;

   public float wallSensorRange = 100f;
   public float groundSensorRange = 1f;
   public RaycastHit frontSensorHit; //Details front sensor detects.
   public bool frontSensorDetected; //True if the front sensor has detected something.
   public RaycastHit rearSensorHit; //Details rear sensor detects.
   public bool rearSensorDetected; //True if the rear sensor has detected something.
   public RaycastHit groundSensorHit; //Details ground sensor detects.
   public bool groundSensorDetected; //True if the ground sensor has detected something.
   public float tileTime = 1.0f; //Time taken for the player to move one tile.
   public float spinTime = 1.0f; //Time taken for the player to rotate 90 degrees.

   private bool run; //If true, run the custom script.
   private string scriptRepresentation; //A visual representation of the script written so far.
   private int indentationLevel = 0; //Indentation level of the script representation.
   private string loopIndexName = ""; //Allows for new loop indexes to be created in the dynamic code by extending the previous one.

   private Ray frontSensor = new Ray(); //Detects walls and other objects ahead of player.
   private Ray rearSensor = new Ray(); //Detects walls and other objects behind the player.
   private Ray groundSensor = new Ray(); //Detects details about the ground below the player.
   private int horizontalSensorDetectable; //Layer for things detectable with the front sensor.
   private int groundSensorDetectable; //Layer for things detectable with the front sensor.

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
      ifEvent.command +=       StartIf;
      loopEvent.command +=     StartLoop;
   }

   void OnDisable() {
      forwardEvent.command -=  AddForwardStep;
      backwardEvent.command -= AddBackwardStep;
      rightEvent.command -=    AddRightwardStep;
      leftEvent.command -=     AddLeftwardStep;
      whileEvent.command -=    StartWhile;
      ifEvent.command -=       StartIf;
      loopEvent.command -=     StartLoop;
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
   Vector3 startPosition;
   Vector3 targetPosition;
   Quaternion startRotation;
   Quaternion targetRotation;
   ";

      scriptBody = "";

      scriptFooter = @"
   yield return null;
   }
}
   ";

      winLoseMessage.text = "";
      run = false;
      scriptRepresentation = "";
      horizontalSensorDetectable = LayerMask.GetMask ("HorizontalSensorDetectable");
      groundSensorDetectable = LayerMask.GetMask ("GroundSensorDetectable");
      frontSensorDetected = false;
      rearSensorDetected = false;
      groundSensorDetected = false;
   }

   // Update is called once per frame
   void Update () {
      frontSensor.origin = transform.position + 0.5f * transform.forward.normalized;
      frontSensor.direction = transform.forward;
      frontSensorDetected = Physics.Raycast (frontSensor, out frontSensorHit, wallSensorRange, horizontalSensorDetectable);

      //Debug.DrawRay (frontSensor.origin, frontSensor.direction * wallSensorRange, Color.red);
      //if (frontSensorDetected) {
      //   scriptPreview.text = "Hit: " + frontSensorHit.distance + "m.\nScript: " + scriptRepresentation;
      //} else {
      //   scriptPreview.text = "Nope.\nScript: " + scriptRepresentation;
      //}

      rearSensor.origin = transform.position;
      rearSensor.direction = -transform.forward;
      rearSensorDetected = Physics.Raycast (rearSensor, out rearSensorHit, wallSensorRange, horizontalSensorDetectable);

      groundSensor.origin = transform.position;
      groundSensor.direction = Vector3.down;
      groundSensorDetected = Physics.Raycast (groundSensor, out groundSensorHit, groundSensorRange, groundSensorDetectable);

      scriptPreview.text = "Script: " + scriptRepresentation;
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
      if( other.gameObject.CompareTag("ColourGreen") )
      {
         winLoseMessage.text = "You win!";
      } else if (other.gameObject.CompareTag("ColourRed") )
      {
         winLoseMessage.text = "Game over";
      }
   }

   //The following functions are controlled by the UI elements to modify and run the custom script.

   public void AddForwardStep(int duration)
   {
      scriptBody += @"
      distance = Math.Min (" + duration + @", (int) Mathf.Round(parent.frontSensorHit.distance));
      ";

      scriptBody += @"
      timer = tileTime * distance;
      startPosition = parent.transform.position;
      targetPosition = startPosition + parent.transform.forward.normalized * distance;

      while(timer > 0f) {
         timer -= Time.deltaTime;
         parent.transform.position = Vector3.Lerp(startPosition, targetPosition, (tileTime * distance - Math.Max(timer, 0)) / (tileTime * distance));
         yield return null;
      }
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
      distance = Math.Min (" + duration + @", (int) Mathf.Round(parent.rearSensorHit.distance));
      ";

      scriptBody += @"
      timer = tileTime * distance;
      startPosition = parent.transform.position;
      targetPosition = startPosition - parent.transform.forward.normalized * distance;

      while(timer > 0f) {
         timer -= Time.deltaTime;
         parent.transform.position = Vector3.Lerp(startPosition, targetPosition, (tileTime * distance - Math.Max(timer, 0)) / (tileTime * distance));
         yield return null;
      }
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
      targetRotation = startRotation * Quaternion.Euler(Vector3.up * 90 * rotations);
      
      while(timer > 0f) {
         timer -= Time.deltaTime;
         parent.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, (spinTime * rotations - Math.Max(timer, 0)) / (spinTime * rotations));
         yield return null;
      }
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
      targetRotation = startRotation * Quaternion.Euler(Vector3.up * -90 * rotations);
      
      while(timer > 0f) {
         timer -= Time.deltaTime;
         parent.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, (spinTime * rotations - Math.Max(timer, 0)) / (spinTime * rotations));
         yield return null;
      }
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Turn left " + (duration * 90).ToString() + " degrees.";
   }

   public void StartWhile(string sensorType, string op, string value) {
      string negation = "!";
      if (op == "=") {
         op = "==";
         negation = "";
      }

      if(sensorType == "Front Sensor Distance") {
         scriptBody += @"
         while(Mathf.Round(parent.frontSensorHit.distance) " + op + " " + value + @") {
         ";
      } else if(sensorType == "Front Sensor Colour") {
         scriptBody += @"
         while(" + negation + @"parent.frontSensorHit.collider.gameObject.CompareTag(""Colour" + value + @""")) {
         ";
      } else if(sensorType == "Ground Sensor Colour") {
         scriptBody += @"
         while(" + negation + @"parent.groundSensorHit.collider.gameObject.CompareTag(""Colour" + value + @""")) {
         ";
      } else {
         indentationLevel -= 1;
      }

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      indentationLevel += 1;

      if(sensorType == "Front Sensor Distance") {
         scriptRepresentation += "While(FrontSensorDistance " + op + " " + value + ") {";
      } else if(sensorType == "Front Sensor Colour") {
         scriptRepresentation += "While(FrontColour is " + (op == "!=" ? "not " : "") + value + ") {";
      } else if(sensorType == "Ground Sensor Colour") {
         scriptRepresentation += "While(GroundColour is " + (op == "!=" ? "not " : "") + value + ") {";
      } else {
         scriptRepresentation += "Unknown sensor type used.";
      }
   }

   public void EndBlock() {
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

   public void StartIf(string sensorType, string op, string value) {
      string negation = "!";
      if (op == "=") {
         op = "==";
         negation = "";
      }

      if(sensorType == "Front Sensor Distance") {
         scriptBody += @"
         if(Mathf.Round(parent.frontSensorHit.distance) " + op + " " + value + @") {
         ";
      } else if(sensorType == "Front Sensor Colour") {
         scriptBody += @"
         if(" + negation + @"parent.frontSensorHit.collider.gameObject.CompareTag(""Colour" + value + @""")) {
         ";
      } else if(sensorType == "Ground Sensor Colour") {
         scriptBody += @"
         if(" + negation + @"parent.groundSensorHit.collider.gameObject.CompareTag(""Colour" + value + @""")) {
         ";
      } else {
         indentationLevel -= 1;
      }

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      indentationLevel += 1;

      if(sensorType == "Front Sensor Distance") {
         scriptRepresentation += "If(FrontSensorDistance " + op + " " + value + ") {";
      } else if(sensorType == "Front Sensor Colour") {
         scriptRepresentation += "If(FrontColour is " + (op == "!=" ? "not " : "") + value + ") {";
      } else if(sensorType == "Ground Sensor Colour") {
         scriptRepresentation += "If(GroundColour is " + (op == "!=" ? "not " : "") + value + ") {";
      } else {
         scriptRepresentation += "Unknown sensor type used.";
      }
   }

   public void StartElse() {
      scriptBody += @"
      } else {
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel - 1; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "} else {";
   }

   public void StartLoop(int numLoops) {
      loopIndexName += "i";

      scriptBody += @"
      for(int " + loopIndexName + @" = 0; " + loopIndexName + @" < " + numLoops + @"; " + loopIndexName + @"++) {
      ";

      scriptRepresentation += "\n";
      for(int i = 0; i < indentationLevel; i++) {
         scriptRepresentation += "   ";
      }
      scriptRepresentation += "Loop " + numLoops + " times {";

      indentationLevel += 1;
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
