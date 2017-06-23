/*
 * Stores information about the command that may be used externally. 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDetails : MonoBehaviour {
   public CommandScriptCode scriptCode; //Holds the code that the actual compiled script will use from this command.
   public CommandDetails footCommandDetails; //Reference to the command details of the foot, if present.
   public float commandIndentation; //Amount the command should be offset to the right, to account for indentation.
   public int numBlocks = 1; //Size of command in terms of how much space it takes up in the visual script.
   public int scriptPosition = 0; //Position within the script. 0 indicates not in script.

   void Start() {
      scriptCode = GetComponent<CommandScriptCode> ();
   }
}
