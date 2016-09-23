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

public class PlayerController : MonoBehaviour
{
    public float               movementSpeed = 1.0f;
    public GameObject          joystick;
    private JoystickController joystickController;

    private Vector3             movementVector  = Vector3.zero;
    private Quaternion          LookRotation    = new Quaternion();
    private Rigidbody           rigidBody;
    private CharacterController controller;
    private PlayerTarget        target          = new PlayerTarget (Vector3.zero, false);

	void Start ()
    {
	    rigidBody          = GetComponent<Rigidbody>();
        controller         = GetComponent<CharacterController>();
        joystickController = joystick.GetComponent<JoystickController>();
	}

	void Update ()
    {}

    void FixedUpdate ()
    {
	    movementVector.x = Input.GetAxis ("Horizontal");
        movementVector.z = Input.GetAxis ("Vertical");

        if (joystickController.InputDirection.magnitude > 0)
        {
            movementVector.x = joystickController.InputDirection.x;
            movementVector.z = joystickController.InputDirection.z;
        }

        MovementInputUpdated();
        UpdateLookRotation();
        transform.rotation = LookRotation;
	}

    void MovementInputUpdated()
    {
        if (movementVector.magnitude > 1.0f)
            movementVector = movementVector.normalized;

        controller.SimpleMove (movementVector * movementSpeed * Time.deltaTime); 
    }

    void UpdateLookRotation()
    {
        if (target.IsValidTarget)
        {
            Vector3 lookVector = target.TargetLocation - transform.position;
            lookVector.y = transform.position.y;
            lookVector = lookVector.normalized;
            LookRotation = Quaternion.LookRotation (lookVector);
        }
        else if (movementVector.magnitude > 0.1f)
        {
            LookRotation = Quaternion.LookRotation(movementVector);
        }
    }

    public void UpdatePlayerTargetPosition (Vector3 targetPosition)
    {
        target = new PlayerTarget (targetPosition, true);
    }

    public void ClearTargetPosition()
    {
        target = new PlayerTarget (Vector3.zero, false);
    }
}
