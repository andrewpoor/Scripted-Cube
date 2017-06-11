using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopScriptController : MonoBehaviour {

   public GlobalScriptController globalScriptController;
   public Transform bodyPiecePrefab; //Clones are to be instantiated as necessary.
   public GameObject foot; //The foot of the block.

   private List<GameObject> bodyPieces; //Indicators of the indentation level, and the block length. Doesn't include the bottom-most one.
   private List<GameObject> commands; //List of all commands. The header is also a command, but isn't in this list.

   private int numLoops = 3; //TEMP
   private string loopIndexIdentifier;

   //Boilerplate code for assembling the block's script.
   private string scriptHeader;
   private string scriptBody;
   private string scriptFooter;

   public void Start() {
      bodyPieces = new List<GameObject> ();
      commands = new List<GameObject> ();

      loopIndexIdentifier = globalScriptController.NewLoopIdentifier ();

      GetComponent<CommandDetails> ().numBlocks = 3;
   }

   public string CollateScripts() {
      scriptHeader = @"
      for(int " + loopIndexIdentifier + @" = 0; " + loopIndexIdentifier + @" < " + numLoops + @"; " + loopIndexIdentifier + @"++) {
         yielded = false;
      ";

      scriptBody = "";
      foreach (GameObject command in commands) {
         scriptBody += command.GetComponent<CommandDetails> ().scriptCode.GetCode ();
      }

      scriptFooter = @"
         if(!yielded) {
            yield return null;
         }
      }
      ";

      return scriptHeader + scriptBody + scriptFooter;
   }
}
