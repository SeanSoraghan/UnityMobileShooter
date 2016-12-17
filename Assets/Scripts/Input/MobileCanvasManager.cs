using UnityEngine;
using System.Collections;

public class MobileCanvasManager : CanvasTouchManager {

    public static bool IsTagValidTarget (string tag)
    {
        return tag == EnemyController.EnemyTag;
    }

    public GameObject   PlayerObject;

    private RaycastHit raycastResult;

    public override void HandleMouseDownEvent (Vector2 mousePosition) { UpdatePlayerTargetFromScreenPosition (Input.mousePosition); }
    public override void HandleMouseDragEvent (Vector2 mousePosition) { UpdatePlayerTargetFromScreenPosition (Input.mousePosition); }

    public override void HandleNewOrExistingTouch (Touch t)
    {
        base.HandleNewOrExistingTouch (t);
        UpdatePlayerTargetFromScreenPosition (t.position);
    }

    private void UpdatePlayerTargetFromScreenPosition (Vector2 screenPosition)
    {
        Vector2[] southWestNorthEastInputPlane = SMath.CreatePlaneFromPoint (screenPosition, 10.0f, 10.0f);
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
}
