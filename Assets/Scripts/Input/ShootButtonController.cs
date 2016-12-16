using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShootButtonController : CanvasTouchHandler
{
    public float screenWidthToControlWidthRatio = 0.1667f;
    public GameObject PlayerObject;

    Image buttonImage;
    private PlayerController Player;

	void Start ()
    {
	    buttonImage = GetComponent<Image>();

        float rectSize = Screen.width * screenWidthToControlWidthRatio;
        float y = GetComponent<RectTransform>().anchoredPosition.y;
        float x = Screen.width - rectSize;
        GetComponent<RectTransform>().anchoredPosition = new Vector2 (x, y);
        GetComponent<RectTransform>().sizeDelta        = new Vector2 (rectSize, rectSize);

        if (PlayerObject != null)
            Player = PlayerObject.GetComponent<PlayerController>();
	}
	
	void Update ()
    {
	
	}

    public override void HandleNewOrExistingTouch (Touch t)
    {
        if (Player != null)
            Player.Shoot();
    }

    public override void HandleTouchEnded (Touch t)
    {}

    public override void HandleMouseDownEvent (Vector2 mousePosition)
    {
        if (Player != null)
            Player.Shoot();
    }

    public override void HandleMouseDragEvent (Vector2 mousePosition) { }
    public override void HandleMouseUpEvent   (Vector2 mousePosition) { }
}
