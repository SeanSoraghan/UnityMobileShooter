using UnityEngine;
using System.Collections;

public class ObjectMover : MonoBehaviour
{
    public enum MoveTriggerType
    {
        Unknown     = 0,
        Environment = 1,
        Player      = 2
    }

    public enum PositionState
    {
        Original    = 0,
        Transformed = 1
    };

    public Transform TargetPosition;
    public float MovementSpeed = 0.01f;
    
    private bool            IsMoving                = false;
    private Vector3         StartPosition           = Vector3.zero;
    private Vector3         EndPosition             = Vector3.zero;
    private float           interpolationTime       = 0.0f;
    private PositionState   ObjectPositionState     = PositionState.Original;
    private MoveTriggerType LastMovementTriggerType = MoveTriggerType.Unknown;

	void Start ()
    {
	    
	}

	void Update ()
    {
	    if (IsMoving)
        {
            float t = interpolationTime * interpolationTime;
            transform.position = Vector3.Lerp (StartPosition, EndPosition, t);

            interpolationTime += MovementSpeed;

            float eps = 0.00001f;
            if (Vector3.Distance (transform.position, EndPosition) < eps)
            { 
                IsMoving = false;
                SwitchStartEndPositions();
                FlipPositionState();
                interpolationTime = 0.0f;
            }
        }
	}

    public bool IsInTransition() { return IsMoving; }
    public bool IsInOrChangingToPositionState(PositionState p)
    {
        if (!IsMoving && ObjectPositionState == p)
            return true;
        if (IsMoving && ObjectPositionState != p) //Assumes only two PositionState values. So will only work if states remain as original & transformed only.
            return true;
        return false;
    }

    public bool IsTransitioningToPositionStateByTriggerType(PositionState p, MoveTriggerType t)
    {
        return IsMoving && ObjectPositionState != p && LastMovementTriggerType == t;
    }

    public bool HasBeenMovedToPositionByTriggerType(PositionState p, MoveTriggerType t)
    {
        return (IsInStaticPosition(p) && LastMovementTriggerType == t) 
               || IsTransitioningToPositionStateByTriggerType (p, t);
    }

    public bool IsInStaticPosition(PositionState p)
    {
        return !IsMoving && ObjectPositionState == p;
    }

    public bool IsInPositionStateOrHasBeenTriggered(PositionState p, MoveTriggerType t)
    {
        return IsInStaticPosition(p) || IsTransitioningToPositionStateByTriggerType(p, t);
    }
    public PositionState GetPositionState() { return ObjectPositionState; }

    public virtual void TriggerMove(MoveTriggerType t)
    {
        if (TargetPosition != null)
        {
            if (!IsMoving)
            { 
                StartPosition = transform.position;
                EndPosition   = TargetPosition.position;
                IsMoving = true;
                LastMovementTriggerType = t;
            }
        }
    }

    private void SwitchStartEndPositions()
    {
        Vector3 temp = StartPosition;
        StartPosition = transform.position;
        TargetPosition.position = temp;
    }

    private void FlipPositionState()
    {
        ObjectPositionState = ObjectPositionState == PositionState.Original ? PositionState.Transformed
                                                                            : PositionState.Original;
    }
}
