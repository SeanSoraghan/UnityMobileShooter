using UnityEngine;
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
}

//================================================================================================
//================================================================================================

public class CanvasTouchManager : CanvasTouchHandler
{
    public GameObject[] CanvasObjects;
    public GameObject   PlayerObject;

    private RaycastHit raycastResult;

    void Start ()
    {}
	
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
        bool eventHandled = false;
        for (int c = 0; c < CanvasObjects.Length; c++)
        {
            if (IsScreenPositionInChildBounds (CanvasObjects[c], Input.mousePosition))
            {
                CanvasTouchHandler handler = CanvasObjects[c].GetComponent<CanvasTouchHandler>();
                if (handler != null)
                {
                    switch (eventType)
                    {
                        case MouseEventType.MouseDown: handler.HandleMouseDownEvent (Input.mousePosition); break;
                        case MouseEventType.MouseDrag: handler.HandleMouseDragEvent (Input.mousePosition); break;
                        case MouseEventType.MouseUp:   handler.HandleMouseUpEvent   (Input.mousePosition); break;
                    }
                    eventHandled = true; 
                }
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

    public override void HandleMouseDownEvent (Vector2 mousePosition) { UpdatePlayerTargetFromScreenPosition (Input.mousePosition); }
    public override void HandleMouseDragEvent (Vector2 mousePosition) { UpdatePlayerTargetFromScreenPosition (Input.mousePosition); }

    private void HandleTouches()
    {
        for (int t = 0; t < Input.touchCount; t++)
        {
            Touch touch = Input.touches[t];
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            { 
                bool touchHandled = ForwardNewOrExistingTouchToChildren (touch);
                if ( ! touchHandled)
                    HandleNewOrExistingTouch (touch);
            }
            else
            {
                bool touchHandled = ForwardTouchEndedToChildren (touch);
                if (! touchHandled)
                    HandleTouchEnded (touch);
            }
        }
    }

    private bool ForwardNewOrExistingTouchToChildren (Touch touch)
    {
        for (int c = 0; c < CanvasObjects.Length; c++)
        { 
            if (IsScreenPositionInChildBounds (CanvasObjects[c], touch.position))
            { 
                CanvasTouchHandler handler = CanvasObjects[c].GetComponent<CanvasTouchHandler>();
                if (handler != null)
                { 
                    handler.HandleNewOrExistingTouch (touch);
                    return true;
                }
            }
        }
        return false;
    }

    private bool ForwardTouchEndedToChildren (Touch touch)
    {
        for (int c = 0; c < CanvasObjects.Length; c++)
        {
            CanvasTouchHandler handler = CanvasObjects[c].GetComponent<CanvasTouchHandler>();
            bool touchInChild = IsScreenPositionInChildBounds (CanvasObjects[c], touch.position);
            if (handler != null && (handler.touchID == touch.fingerId || (handler.touchID == -1 && touchInChild)))
            {
                handler.HandleTouchEnded (touch);
                return true;
            }
        }
        return false;
    }
    
    public override void HandleNewOrExistingTouch (Touch t)
    {
        base.HandleNewOrExistingTouch (t);
        UpdatePlayerTargetFromScreenPosition (t.position);
    }

    private void UpdatePlayerTargetFromScreenPosition (Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay (screenPosition);
        if (Physics.Raycast (ray, out raycastResult))
        {
            PlayerController controller = PlayerObject.GetComponent<PlayerController>();
            if (controller != null)
                controller.UpdatePlayerTargetPosition (raycastResult.point);
        }
    }

    private bool IsScreenPositionInChildBounds (GameObject childElement, Vector2 touchScreenPosition)
    {
        if (childElement == null)
            return false;

        RectTransform childRectTrasform = childElement.GetComponent<RectTransform>();
        if (childRectTrasform == null)
            return false;

        return childRectTrasform.rect.Contains (touchScreenPosition);
    }
}
