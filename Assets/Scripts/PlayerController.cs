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
    public float                movementSpeed = 1.0f;
    public GameObject           joystick;
    public GameObject           Gunshot;

    private JoystickController  joystickController;
    private Vector3             movementVector  = Vector3.zero;
    private Quaternion          LookRotation    = new Quaternion();
    private CharacterController controller;
    private PlayerTarget        target          = new PlayerTarget (Vector3.zero, false);
    private RaycastHit          shootRaycastResult;
    private float               shotDamage = 10.0f;

	void Start ()
    {
        controller         = GetComponent<CharacterController>();
        joystickController = joystick.GetComponent<JoystickController>();
	}

	void Update ()
    {

    }

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

    public void Shoot()
    {
        if (target.IsValidTarget)
        { 
            Vector3 shootRayOrigin = transform.position;
            shootRayOrigin.y = shootRayOrigin.y + transform.localScale.y / 2;
            Vector3 shootRayDirection = (target.TargetLocation - shootRayOrigin).normalized;
            if (Physics.Raycast (shootRayOrigin, shootRayDirection, out shootRaycastResult))
            {
                Vector3 debugRayLength = shootRaycastResult.point - shootRayOrigin;
                Debug.DrawRay (shootRayOrigin, debugRayLength, new Color (1.0f, 0.0f, 0.0f), 0.2f, true);

                
                if (Gunshot != null)
                { 
                    Object gunshot = Instantiate (Gunshot, shootRayOrigin, transform.rotation);
                    GameObject gunshotObject = (GameObject) gunshot;
                    gunshotObject.GetComponent<ParticleSystem>().Play();
                    Destroy (gunshotObject, gunshotObject.GetComponent<ParticleSystem>().duration + 0.5f);
                }

                GameObject objectHit = shootRaycastResult.collider.gameObject;
                ActorController actor = objectHit.GetComponent<ActorController>();
                if (actor != null)
                    actor.TakeHit (shootRaycastResult.point, shotDamage);
            }
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
