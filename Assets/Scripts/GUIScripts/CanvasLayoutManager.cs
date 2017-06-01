using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLayoutManager : MonoBehaviour {

   //The various UI elements on screen.
   public RawImage levelPane;
   public Image scriptPane;
   public Image commandsPane;

   //Scale of the level pane.
   public float levelX;
   public float levelY;

   public float borderWidth;

   void Start () {
      Vector2 canvasSize = GetComponent<RectTransform> ().sizeDelta;

      levelPane.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasSize.x * levelX, canvasSize.y * levelY);
      scriptPane.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasSize.x * (1.0f - levelX) - 5.0f * borderWidth, canvasSize.y - 2.0f * borderWidth);
      scriptPane.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (borderWidth, 0.0f);
      commandsPane.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasSize.x * levelX, canvasSize.y * (1.0f - levelY) - borderWidth);
   }
}
