using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class JoystickController : CanvasTouchHandler
{
    public float screenWidthToControlWidthRatio = 0.1667f;
    public float innerJoystickSizeRatio         = 0.5f;
    public Vector3 InputDirection { set; get; }

    Image joystickPad;
    Image joystick;

	void Start ()
    {
	    joystickPad = GetComponent<Image>();
        joystick    = transform.GetChild(0).GetComponent<Image>();

        InputDirection = Vector3.zero;
        float rectSize = Screen.width * screenWidthToControlWidthRatio;
        GetComponent<RectTransform>().sizeDelta = new Vector2 (rectSize, rectSize);
        float innerRectSize = rectSize * innerJoystickSizeRatio;
        joystick.rectTransform.sizeDelta = new Vector2 (innerRectSize, innerRectSize);
	}
	
	void Update ()
    {
	
	}
    public override void HandleMouseDownEvent (Vector2 mousePosition) { PointerPositionUpdated (mousePosition); }
    public override void HandleMouseDragEvent (Vector2 mousePosition) { PointerPositionUpdated (mousePosition); }
    public override void HandleMouseUpEvent   (Vector2 mousePosition) { endTouch(); }

    public override void HandleNewOrExistingTouch (Touch t)
    {
        if (IncomingTouchIsDifferentThanExistingTouch (t))
            return;

        touchID = t.fingerId;
        if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Moved)
            PointerPositionUpdated (t.position);
    }

    private bool IncomingTouchIsDifferentThanExistingTouch (Touch t)
    {
        return touchID != -1 && t.fingerId != touchID;
    }

    public override void HandleTouchEnded (Touch t)
    {
        if ((touchID == t.fingerId || touchID == -1) && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled))
            endTouch();
    }

    private void PointerPositionUpdated (Vector2 pointerPosition)
    {
        Rect rect = GetComponent<RectTransform>().rect;
        if (rect != null && rect.Contains (pointerPosition))
        {
            float x = (pointerPosition.x / rect.size.x) * 2.0f - 1.0f;
            float y = (pointerPosition.y / rect.size.y) * 2.0f - 1.0f;
            InputDirection = new Vector3 (x, 0, y);
            LimitInputAndUpdateJoystickPosition();
        }
    }

    private void LimitInputAndUpdateJoystickPosition()
    {
        InputDirection = InputDirection.magnitude > 1.0f ? InputDirection.normalized : InputDirection;
        float joystickMovementLimitRatio = innerJoystickSizeRatio * 0.5f;
        joystick.rectTransform.anchoredPosition = new Vector3 (InputDirection.x * (joystickPad.rectTransform.sizeDelta.x * joystickMovementLimitRatio),
                                                               InputDirection.z * (joystickPad.rectTransform.sizeDelta.y * joystickMovementLimitRatio),
                                                               0.0f);
    }

    private void endTouch()
    {
        InputDirection = Vector3.zero;
        LimitInputAndUpdateJoystickPosition();
        touchID = -1;
    }
}
