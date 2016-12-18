using UnityEngine;
using System.Collections;

public struct PlayerTarget
{ 
    public PlayerTarget (GameObject t)
    {
        TargetObject  = t;
    }

    public Vector3 GetTargetLocation()
    {
        ActorController actor = ObjectUtils.GetActorControllerFromObject (TargetObject);

        if (actor != null)
            return actor.GetTargetLocation();

        return Vector3.zero;
    }

    public bool IsTargetValid()
    {
        return TargetObject != null;
    }

    public GameObject TargetObject;
}

//================================================================================================
//================================================================================================

public class PlayerController : ActorController
{
    
    public GameObject           Joystick;
    public Transform            WeaponSlot;
    public float                MaxAnimationSpeed           = 1.0f;
    
    public static string        PlayerTag                   = "Player";
    public static string        AnimatorAimingParamString   = "Aiming";
    public static string        AnimatorCarryingParamString = "Carrying";

    private JoystickController  JoystickController;
    private Animator            AnimationController;
    private CharacterController Controller;
    private PlayerTarget        Target                    = new PlayerTarget (null);
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
        if (Input.GetKeyDown ("space"))
            Shoot();
    }

    void FixedUpdate ()
    {
        ActorFixedUpdate();

	    MovementVector.x = Input.GetAxis ("Horizontal");
        MovementVector.z = Input.GetAxis ("Vertical");

        if (JoystickController != null && JoystickController.InputDirection.magnitude > 0)
        {
            MovementVector.x = JoystickController.InputDirection.x;
            MovementVector.z = JoystickController.InputDirection.z;
        }

        MovementInputUpdated();
        UpdateLookRotation();
        transform.rotation = LookRotation;
        UpdateAnimatorRotationDelta(); 
	}

    public void Footstep()
    {
        AkMovementTrigger trigger = GetComponent<AkMovementTrigger>();
        if (trigger != null)
            trigger.TriggerSound();
    }

    void MovementInputUpdated()
    {
        if (MovementVector.magnitude > 1.0f)
            MovementVector = MovementVector.normalized;

        AkSoundEngine.SetRTPCValue ("CharacterSpeed", MovementVector.magnitude * 100.0f);
        Controller.SimpleMove ((MovementVector + StaggerMovementVector) * MovementSpeed);
        if (AnimationController != null) 
            AnimationController.SetFloat (VelocityMagnitude, MovementVector.magnitude);
    }

    void UpdateLookRotation()
    {
        if (Target.IsTargetValid())
        {
            Vector3 lookVector = Target.GetTargetLocation() - transform.position;
            lookVector.y = transform.position.y;
            lookVector = lookVector.normalized;
            LookRotation = Quaternion.LookRotation (lookVector);
            AnimationController.SetLookAtPosition (Target.GetTargetLocation());
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
        //if (rotationDelta < -0.75f)
        //    rotationDelta += 2.0f;

        AnimationController.SetFloat (LookMovementRotationDelta, rotationDelta);
        //float animationSpeed = MovementVector.magnitude * MaxAnimationSpeed;
        //if (animationSpeed == 0.0f)
        //    animationSpeed = 1.0f; //idle animation.
        //AnimationController.speed = animationSpeed;
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
        if (Target.IsTargetValid())
        {
            Vector3 targetLocation = Target.GetTargetLocation();
            Shoot (targetLocation);
        }

        UpdateAnimationParameters();
    }

    public void UpdatePlayerTargetPosition (GameObject targetObject)
    {
        Target = new PlayerTarget (targetObject);
        UpdateAnimationParameters();
    }

    public void ClearTargetPosition()
    {
        Target = new PlayerTarget (null);
        UpdateAnimationParameters();
    }

    private void UpdateAnimationParameters()
    {
        SetAnimatorAiming   (false);
        SetAnimatorCarrying (false);

        bool TargetValid = Target.IsTargetValid ();
        if (TargetValid)
        { 
            ActorController actor = ObjectUtils.GetActorControllerFromObject (Target.TargetObject);
            if (actor != null)
                TargetValid = actor.IsAlive;
        }

        if (Gun != null)
        {
            if (TargetValid)
                SetAnimatorAiming (true);
            else
                SetAnimatorCarrying (true);
        }
    }

    private void SetAnimatorAiming (bool aiming)
    {
        AnimationController.SetBool (AnimatorAimingParamString, aiming);
    }

    private void SetAnimatorCarrying (bool carrying)
    {
        AnimationController.SetBool (AnimatorCarryingParamString, carrying);
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

            UpdateAnimationParameters();
        }
    }
}
