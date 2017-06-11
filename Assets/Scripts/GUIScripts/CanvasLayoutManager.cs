using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLayoutManager : MonoBehaviour {

   //The various UI elements on screen.
   public RawImage levelPane;
   public Image scriptPane;
   public Image commandsPane;
   public Image scrollBar;
   public Image runPane;

   //Scale of certain elements.
   public float levelX;
   public float levelY;
   public float scriptPaneY;
   public float tabsX;

   public float borderWidth;

   void Start () {
      Vector2 canvasSize = GetComponent<RectTransform> ().sizeDelta;

      levelPane.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasSize.x * levelX - borderWidth, canvasSize.y * levelY - borderWidth);
      levelPane.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (-borderWidth, -borderWidth);

      scriptPane.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasSize.x * (1.0f - levelX) - 10.0f * borderWidth, canvasSize.y * scriptPaneY - 2.0f * borderWidth);
      scriptPane.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (borderWidth, -borderWidth);

      commandsPane.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasSize.x * levelX - borderWidth, canvasSize.y * (1.0f - levelY) - 2.0f * borderWidth);
      commandsPane.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (-borderWidth, borderWidth);
      commandsPane.GetComponent<CommandsPaneDetails> ().commandsHolder.GetComponent<RectTransform> ().sizeDelta = 
         new Vector2 (commandsPane.GetComponent<RectTransform> ().sizeDelta.x * (1.0f - tabsX), commandsPane.GetComponent<RectTransform> ().sizeDelta.y);
      commandsPane.GetComponent<CommandsPaneDetails> ().tabsHolder.GetComponent<RectTransform> ().sizeDelta = 
         new Vector2 (commandsPane.GetComponent<RectTransform> ().sizeDelta.x * tabsX, commandsPane.GetComponent<RectTransform> ().sizeDelta.y);

      scrollBar.GetComponent<RectTransform> ().sizeDelta = new Vector2 (
         canvasSize.x - levelPane.GetComponent<RectTransform> ().sizeDelta.x - scriptPane.GetComponent<RectTransform> ().sizeDelta.x - 4.0f * borderWidth, 
         canvasSize.y * scriptPaneY - 2.0f * borderWidth);
      scrollBar.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (scriptPane.GetComponent<RectTransform> ().sizeDelta.x + 2.0f * borderWidth, -borderWidth);

      runPane.GetComponent<RectTransform> ().sizeDelta = new Vector2(
         scriptPane.GetComponent<RectTransform> ().sizeDelta.x + scrollBar.GetComponent<RectTransform> ().sizeDelta.x + borderWidth,
         canvasSize.y * (1.0f - scriptPaneY) - borderWidth);
      runPane.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (borderWidth, borderWidth);
   }
}
