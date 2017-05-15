using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

////The dynamically loaded scripts must inherit from this, to provide a public API.
//public interface DynamicPlayerController {
//   IEnumerator GetCoroutine(PlayerController parent); //Returns the coroutine that holds the scripted actions.
//}

public class PlayerControllerOld : MonoBehaviour {
//   public Text winLoseMessage;
//   public Text scriptPreview;
//
//   private bool run; //If true, run the custom script.
//   private Vector3 startPosition;
//   private Quaternion startRotation;
//   private string scriptRepresentation; //A visual representation of the script written so far.
//   private CarController carController;
//
//   //For dynamically loading the user's script.
//   private System.Reflection.Assembly movementAssembly;
//   private string scriptPath;
//   private IEnumerator scriptCoroutine;
//
//   //Boilerplate code for assembling the user's script.
//   private string scriptHeader = @"
//using UnityEngine;
//using System.Collections;
//
//public class ScriptedPlayerController : DynamicPlayerController
//{
//   public IEnumerator GetCoroutine(PlayerController parent) {
//      return CustomScript(parent);
//   }
//
//   public IEnumerator CustomScript(PlayerController parent) {
//      float force = -550.0f;
//      var rb = parent.GetRigidBody();
//
//   ";
//
//   private string scriptBody = "";
//
//   private string scriptFooter = @"
//   }
//}
//   ";
//
//   // Use this for initialization
//   void Start () {
//      winLoseMessage.text = "";
//      run = false;
//      startPosition = transform.localPosition;
//      startRotation = transform.localRotation;
//      scriptRepresentation = "";
//      carController = GetComponentInChildren<CarController> ();
//   }
//
//   // Update is called once per frame
//   void Update () {
//      scriptPreview.text = "Script: " + scriptRepresentation;
//   }
//
//   // FixedUpdate is called once per frame, before physics calculations
//   void FixedUpdate() {
//      //Once only, start the user's script.
//      //The script will continue every frame until completion.
//      if (run) {
//         run = false; //Only run one instance of the script at a time.
//
//         IEnumerator script = BeginScript ();
//         StartCoroutine(script);
//      }
//   }
//
//   IEnumerator BeginScript()
//   {
//      //Wait for a moment first, to avoid visual frame issues.
//      yield return new WaitForSeconds (0.5f);
//
//      //Run the user's script.
//      StartCoroutine(scriptCoroutine);
//      yield return null;
//   }
//
//   void OnTriggerEnter( Collider other )
//   {
//      if( other.gameObject.CompareTag("Succeed") )
//      {
//         winLoseMessage.text = "You win!";
//      } else if (other.gameObject.CompareTag("Fail") )
//      {
//         winLoseMessage.text = "Game over";
//      }
//   }
//
//   private void ResetPlayer()
//   {
//      transform.localPosition = startPosition;
//      transform.localRotation = startRotation;
//   }
//
//   //The following functions are controlled by the UI elements to modify and run the custom script.
//
//   public void AddForwardStep()
//   {
//      scriptBody += @"
//      while(rb.velocity.magnitude > 0.0f) {
//         yield return null;
//      }      
//      rb.AddForce (new Vector3(0.0f, 0.0f, force));      
//      while(rb.velocity.magnitude <= 0.0f) {
//         yield return null;
//      }
//
//      ";
//
//      scriptRepresentation += "↑ ";
//   }
//
//   public void AddBackwardStep()
//   {
//      scriptBody += @"
//      while(rb.velocity.magnitude > 0.0f) {
//         yield return null;
//      }      
//      rb.AddForce (new Vector3(0.0f, 0.0f, -force));      
//      while(rb.velocity.magnitude <= 0.0f) {
//         yield return null;
//      }
//
//      ";
//
//      scriptRepresentation += "↓ ";
//   }
//
//   public void AddRightwardStep()
//   {
//      scriptBody += @"
//      while(rb.velocity.magnitude > 0.0f) {
//         yield return null;
//      }      
//      rb.AddForce (new Vector3(force, 0.0f, 0.0f));      
//      while(rb.velocity.magnitude <= 0.0f) {
//         yield return null;
//      }
//
//      ";
//
//      scriptRepresentation += "→ ";
//   }
//
//   public void AddLeftwardStep()
//   {
//      scriptBody += @"
//      while(rb.velocity.magnitude > 0.0f) {
//         yield return null;
//      }      
//      rb.AddForce (new Vector3(-force, 0.0f, 0.0f));      
//      while(rb.velocity.magnitude <= 0.0f) {
//         yield return null;
//      }
//
//      ";
//
//      scriptRepresentation += "← ";
//   }
//    
//   //Reset all of the game's components
//   public void ResetScript()
//   {
//      run = false;
//      scriptBody = "";
//      ResetPlayer ();
//      StopCoroutine (scriptCoroutine);
//      winLoseMessage.text = "";
//      scriptRepresentation = "";
//   }
//
//   //Compile and run the script generated by the user
//   public void RunScript()
//   {
//      if(scriptBody != "") {
//         //      scriptPath = Application.dataPath + "/playerScript.txt";
//         //      var scriptFile = File.CreateText(scriptPath);
//         //      scriptFile.Write(script);
//         //      scriptFile.Close();
//
//         //      script = RuntimeCompiler.Load(scriptPath);
//         //
//         //      if (script == null)
//         //      {
//         //         throw new FileLoadException();
//         //      }
//
//         string script = scriptHeader + scriptBody + scriptFooter;
//
//         //Compile the script and extract its coroutine, which contains the actions for the player to perform.
//         movementAssembly = RuntimeCompiler.Compile(script);
//         DynamicPlayerController dpc = (DynamicPlayerController) movementAssembly.CreateInstance("ScriptedPlayerController");
//         scriptCoroutine = dpc.GetCoroutine (this);
//
//         run = true;
//      }
//   }
}
