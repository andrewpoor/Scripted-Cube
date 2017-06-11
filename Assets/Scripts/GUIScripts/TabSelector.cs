using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabSelector : MonoBehaviour {

   //When this is selected, display it instead of whatever tab was currently being shown.
   public void SelectTab() {
      transform.SetAsLastSibling();
   }
}
