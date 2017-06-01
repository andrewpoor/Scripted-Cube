using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLayoutManager : MonoBehaviour {

   void Start () {
      BoxCollider2D boxCollider = GetComponent<BoxCollider2D> ();
      boxCollider.size = GetComponent<RectTransform>().sizeDelta;
   }
}
