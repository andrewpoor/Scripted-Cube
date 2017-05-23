using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

   public void LoadLevel(int level)
   {
      string sceneName = "Level" + level;
      if (level == 0) {
         sceneName = "DemoLevel";
      }
      SceneManager.LoadScene(sceneName);
   }
}
