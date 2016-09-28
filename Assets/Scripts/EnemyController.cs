using UnityEngine;
using System.Collections;

public class EnemyController : ActorController
{
    public float     SightDistance   = 100.0f;
    public float     AttackDistance  = 50.0f;
    public float     Damping         = 0.9f;
    public Transform PlayerTransform;

    private CharacterController Controller;
    private float               PlayerTargetDistance;
    private RaycastHit          ShootRaycastResult;
    

	void Start ()
    {
        Initialise();
        Controller = GetComponent<CharacterController>();

	}
	
	void FixedUpdate ()
    {
        if (PlayerTransform != null)
        { 
	        PlayerTargetDistance = Vector3.Distance (PlayerTransform.position, transform.position);
            if (PlayerTargetDistance < SightDistance)
            {
                UpdateLookRotation();
                transform.rotation = LookRotation;
            }
            if (PlayerTargetDistance < AttackDistance)
                Shoot();
        }
	}

    void UpdateLookRotation()
    {
        if (PlayerTransform != null)
        {
            Vector3 LookVector = PlayerTransform.position - transform.position;
            LookVector.y = 0.0f;
            LookRotation = Quaternion.LookRotation (LookVector);
        }
    }

    public void Shoot()
    {
        Vector3 lookVector = transform.forward.normalized;
        Shoot (lookVector);
    }
}
