using UnityEngine;
using System.Collections;

public struct PlayerTarget
{ 
    public PlayerTarget (Vector3 t, bool v)
    {
        TargetLocation = t;
        IsValidTarget  = v;
    }

    public Vector3 TargetLocation;
    public bool    IsValidTarget;
}

//================================================================================================
//================================================================================================

public class PlayerController : ActorController
{
    
    public GameObject           Joystick;
    public Transform            WeaponSlot;
    public float                MaxAnimationSpeed = 1.0f;
    public string               AnimatorAimingParamString = "Aiming";

    private JoystickController  JoystickController;
    private Animator            AnimationController;
    private CharacterController Controller;
    private PlayerTarget        Target                    = new PlayerTarget (Vector3.zero, false);
    private string              LookMovementRotationDelta = "LookMovementRotationDelta";
    private string              VelocityMagnitude         = "VelocityMagnitude";
    private Quaternion          DefaultArmRotation        = Quaternion.identity;

	void Start ()
    {
        Initialise();
        Controller          = GetComponent<CharacterController>();
        JoystickController  = Joystick.GetComponent<JoystickController>();
        AnimationController = GetComponent<Animator>();
	}

    void Update()
    {
        UpdateActor();
    }

    void FixedUpdate ()
    {
	    MovementVector.x = Input.GetAxis ("Horizontal");
        MovementVector.z = Input.GetAxis ("Vertical");

        if (JoystickController.InputDirection.magnitude > 0)
        {
            MovementVector.x = JoystickController.InputDirection.x;
            MovementVector.z = JoystickController.InputDirection.z;
        }

        MovementInputUpdated();
        UpdateLookRotation();
        transform.rotation = LookRotation;
        UpdateAnimatorRotationDelta();
	}

    void MovementInputUpdated()
    {
        if (MovementVector.magnitude > 1.0f)
            MovementVector = MovementVector.normalized;

        Controller.SimpleMove (MovementVector * MovementSpeed * Time.deltaTime);
        if (AnimationController != null) 
            AnimationController.SetFloat (VelocityMagnitude, MovementVector.magnitude);
    }

    void UpdateLookRotation()
    {
        if (Target.IsValidTarget)
        {
            Vector3 lookVector = Target.TargetLocation - transform.position;
            lookVector.y = transform.position.y;
            lookVector = lookVector.normalized;
            LookRotation = Quaternion.LookRotation (lookVector);
            AnimationController.SetLookAtPosition (Target.TargetLocation);
        }
        else if (MovementVector.magnitude > 0.1f)
        {
            LookRotation = Quaternion.LookRotation(MovementVector);
        }   
    }

    private void UpdateAnimatorRotationDelta ()
    {
        Vector3 lookVector = /*LookRotation * */transform.forward; 
        Quaternion lookMovementRotationDelta = Quaternion.FromToRotation (lookVector, MovementVector);
        Vector3 eulerDelta = lookMovementRotationDelta.eulerAngles;
        float eulerDeltaY = eulerDelta.y;
        float rotationDelta = NormaliseAngle (eulerDeltaY);

        //Hack around non OR conditionals in animator controller
        //instead of x > 0.75f || x < -0.75f, we just convert when x < -0.75, so that we cover the range 0.75 -> 1.25.
        if (rotationDelta < -0.75f)
            rotationDelta += 2.0f;

        AnimationController.SetFloat (LookMovementRotationDelta, rotationDelta);
        float animationSpeed = MovementVector.magnitude * MaxAnimationSpeed;
        if (animationSpeed == 0.0f)
            animationSpeed = 1.0f; //idle animation.
        AnimationController.speed = animationSpeed;
    }

    public static float NormaliseAngle (float angle)
    {
        float fullNegToFullPos = angle;
        
        if (fullNegToFullPos < -360.0f)
            fullNegToFullPos = -Mathf.Repeat (Mathf.Abs (fullNegToFullPos), 360.0f);
        else if (fullNegToFullPos > 360.0f)
            fullNegToFullPos = Mathf.Repeat (fullNegToFullPos, 360.0f);

        float halfNegToHalfPos = fullNegToFullPos;
        if (halfNegToHalfPos < -180.0f)
            halfNegToHalfPos += 360.0f;
        else if (halfNegToHalfPos > 180.0f)
            halfNegToHalfPos -= 360.0f;

        return halfNegToHalfPos / 180.0f;
    }

    public void Shoot ()
    {
        if (Target.IsValidTarget)
            Shoot (Target.TargetLocation);
    }

    public void UpdatePlayerTargetPosition (Vector3 targetPosition)
    {
        Target = new PlayerTarget (targetPosition, true);
        SetAnimatorAiming (true);
    }

    public void ClearTargetPosition()
    {
        Target = new PlayerTarget (Vector3.zero, false);
        SetAnimatorAiming (false);
    }

    private void SetAnimatorAiming (bool aiming)
    {
        AnimationController.SetBool (AnimatorAimingParamString, aiming);
    }

    void OnControllerColliderHit (ControllerColliderHit col)
    {
        if (col.gameObject.tag == GunController.GunTag)
        {
            Debug.Log ("Gun pick up"); 
            PickUpGun (col.gameObject);
            if (Gun != null && WeaponSlot != null)
            { 
                Gun.transform.SetParent (WeaponSlot.transform);
                Gun.transform.localPosition = Vector3.zero;
                Gun.transform.localRotation = Quaternion.Euler (-90.0f, 0.0f, 0.0f);
                Gun.transform.localScale    = new Vector3 (0.001f, 0.001f, 0.001f);
            }
        }
    }
}
