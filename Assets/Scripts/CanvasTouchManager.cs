using UnityEngine;
using UnityEngine.UI;
using System.Collections;

enum MouseEventType
{
    MouseDown = 0,
    MouseDrag,
    MouseUp
};

//================================================================================================
//================================================================================================

public class CanvasTouchHandler : MonoBehaviour
{
    public int touchID;

    public CanvasTouchHandler()
    {
        touchID = -1;
    }

    public virtual void HandleNewOrExistingTouch (Touch t)
    {}

    public virtual void HandleTouchEnded (Touch t)
    {}

    public virtual void HandleMouseDownEvent (Vector2 mousePosition) { }
    public virtual void HandleMouseDragEvent (Vector2 mousePosition) { }
    public virtual void HandleMouseUpEvent   (Vector2 mousePosition) { }

    public bool HasMouseDown;
}

//================================================================================================
//================================================================================================

public class CanvasTouchManager : CanvasTouchHandler
{
    public static bool IsTagValidTarget (string tag)
    {
        return tag == EnemyController.EnemyTag;
    }

    public GameObject[] CanvasObjects;
    public GameObject   PlayerObject;

    private RaycastHit raycastResult;

    void Start ()
    {
    }

	void Update ()
    {
        if (Input.touchCount > 0)
	        HandleTouches();
        if (Input.GetMouseButtonDown (0))
            HandleMouseEvent (MouseEventType.MouseDown);
        if (Input.GetMouseButton (0))
            HandleMouseEvent (MouseEventType.MouseDrag);
        if (Input.GetMouseButtonUp (0))
            HandleMouseEvent (MouseEventType.MouseUp);
	}

    private void HandleMouseEvent (MouseEventType eventType)
    {
        if (SystemInfo.deviceType != DeviceType.Handheld)
        { 
            bool eventHandled = false;
            for (int c = 0; c < CanvasObjects.Length; c++)
            {
                CanvasTouchHandler handler = CanvasObjects[c].GetComponent<CanvasTouchHandler>();
                if (handler != null && (IsScreenPositionInChildBounds (CanvasObjects[c], Input.mousePosition) || handler.HasMouseDown))
                {
                    switch (eventType)
                    {
                        case MouseEventType.MouseDown: handler.HandleMouseDownEvent (Input.mousePosition); break;
                        case MouseEventType.MouseDrag: handler.HandleMouseDragEvent (Input.mousePosition); break;
                        case MouseEventType.MouseUp:   handler.HandleMouseUpEvent   (Input.mousePosition); break;
                    }
                    eventHandled = true; 
                    break;
                }
            }
            if ( ! eventHandled)
            { 
                switch (eventType)
                {
                    case MouseEventType.MouseDown: HandleMouseDownEvent (Input.mousePosition); break;
                    case MouseEventType.MouseDrag: HandleMouseDragEvent (Input.mousePosition); break;
                    case MouseEventType.MouseUp:   HandleMouseUpEvent   (Input.mousePosition); break;
                }
            }
        }
    }

    public override void HandleMouseDownEvent (Vector2 mousePosition) { UpdatePlayerTargetFromScreenPosition (Input.mousePosition); }
    public override void HandleMouseDragEvent (Vector2 mousePosition) { UpdatePlayerTargetFromScreenPosition (Input.mousePosition); }

    private void HandleTouches()
    {
        for (int t = 0; t < Input.touchCount; t++)
        {
            Touch touch = Input.touches[t];
            bool isNewOrExistingTouch = (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary);
            bool touchHandled = ForwardTouchToChildren (touch, isNewOrExistingTouch);
            if ( ! touchHandled)
                HandleTouch (touch, isNewOrExistingTouch);
        }
    }

    private bool ForwardTouchToChildren (Touch touch, bool isNewOrExistingTouch)
    {
        for (int c = 0; c < CanvasObjects.Length; c++)
        {
            CanvasTouchHandler handler = CanvasObjects[c].GetComponent<CanvasTouchHandler>();

            bool touchInChild = IsScreenPositionInChildBounds (CanvasObjects[c], touch.position);
            if (handler != null && (handler.touchID == touch.fingerId || (handler.touchID == -1 && touchInChild)))
            {
                if (isNewOrExistingTouch)
                    handler.HandleNewOrExistingTouch (touch);
                else
                    handler.HandleTouchEnded (touch);
                return true;
            }
        }
        return false;
    }
    
    public void HandleTouch (Touch t, bool isNewOrExistingTouch)
    {
        if(isNewOrExistingTouch)
            HandleNewOrExistingTouch (t);
        else
            HandleTouchEnded (t);
    }

    public override void HandleNewOrExistingTouch (Touch t)
    {
        base.HandleNewOrExistingTouch (t);
        UpdatePlayerTargetFromScreenPosition (t.position);
    }

    private void UpdatePlayerTargetFromScreenPosition (Vector2 screenPosition)
    {
        if (PlayerObject != null)
        { 
            PlayerController controller = PlayerObject.GetComponent<PlayerController>();
            Ray ray = Camera.main.ScreenPointToRay (screenPosition);
            if (Physics.Raycast (ray, out raycastResult))
            {
                if (IsTagValidTarget (raycastResult.collider.gameObject.tag))
                { 
                    if (controller != null)
                    { 
                        controller.UpdatePlayerTargetPosition (raycastResult.collider.gameObject);
                        return;
                    }
                }
            }
            if (controller != null)
                controller.ClearTargetPosition();
        }
    }

    private bool IsScreenPositionInChildBounds (GameObject childElement, Vector2 touchScreenPosition)
    {
        if (childElement == null)
            return false;

        RectTransform childRectTrasform = childElement.GetComponent<RectTransform>();
        if (childRectTrasform == null)
            return false;

        Vector2 p = childRectTrasform.anchoredPosition;
        Rect rect = new Rect (p.x, p.y, childRectTrasform.sizeDelta.x, childRectTrasform.sizeDelta.y);
        return rect.Contains (touchScreenPosition);
    }
}
