using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonController : CanvasTouchHandler
{
    public PlayerController Player;

    Image buttonImage;

	// Use this for initialization
	void Start ()
    {
		buttonImage = GetComponent<Image>();

        float rectSize = Screen.width * screenWidthToControlWidthRatio;
        float y = rectSize;
        float x = Screen.width - rectSize;
        GetComponent<RectTransform>().anchoredPosition = new Vector2 (x, y);
        GetComponent<RectTransform>().sizeDelta        = new Vector2 (rectSize, rectSize);
	}
	
	public override void HandleNewOrExistingTouch (Touch t)
    {
        if (Player != null)
            Player.Action();
    }

    public override void HandleTouchEnded (Touch t)
    {}

    public override void HandleMouseDownEvent (Vector2 mousePosition)
    {
        if (Player != null)
            Player.Action();
    }

    public override void HandleMouseDragEvent (Vector2 mousePosition) { }
    public override void HandleMouseUpEvent   (Vector2 mousePosition) { }
}
