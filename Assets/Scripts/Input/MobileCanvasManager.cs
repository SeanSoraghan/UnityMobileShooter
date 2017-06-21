using UnityEngine;
using System.Collections;

public class MobileCanvasManager : CanvasTouchManager {

    public float  InputRefreshRate        = 0.2f;
    private float previousInputUpdateTime = 0.0f;

    public static bool IsTagValidTarget (string tag)
    {
        return tag == EnemyController.EnemyTag;
    }

    public GameObject   PlayerObject;

    private RaycastHit raycastResult;

    public override void HandleMouseDownEvent (Vector2 mousePosition) { UpdatePlayerTargetFromScreenInputPosition (Input.mousePosition); }
    public override void HandleMouseDragEvent (Vector2 mousePosition) {  }

    public override void HandleNewOrExistingTouch (Touch t)
    {
        base.HandleNewOrExistingTouch (t);
        UpdatePlayerTargetFromScreenInputPosition (t.position);
    }

    private bool ScreenPointChangesPlayerTarget (Vector2 screenPoint, PlayerController controller)
    {
        
        Ray ray = Camera.main.ScreenPointToRay (screenPoint);
        if (Physics.Raycast (ray, out raycastResult))
        {
            if (IsTagValidTarget (raycastResult.collider.gameObject.tag))
            { 
                if (controller != null)
                { 
                    controller.UpdatePlayerTargetPosition (raycastResult.collider.gameObject);
                    return true;
                }
            }
        }

        return false;
    }

    private void UpdatePlayerTargetFromScreenInputPosition (Vector2 screenPosition)
    {
        if (Time.time - previousInputUpdateTime > InputRefreshRate)
        { 
            float inputPlaneWidth  = 40.0f;
            float inputPlaneHeight = 40.0f;
            Vector2[] inputPoints = SMath.GetDiscretePlanePointsFromCentrePoint (screenPosition, inputPlaneWidth, inputPlaneHeight, 10);
            if (PlayerObject != null)
            { 
                PlayerController controller = PlayerObject.GetComponent<PlayerController>();
                Vector2 screenPoint = screenPosition;
                for (int i = 0; i < inputPoints.Length; ++i)
                { 
                    if (ScreenPointChangesPlayerTarget (inputPoints[i], controller))
                        return;
                }

                if (controller != null)
                    controller.ClearTargetPosition();
            }
            previousInputUpdateTime = Time.time;
        }
    }
}
