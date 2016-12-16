using UnityEngine;
using System.Collections;

public class EnemyController : ActorController
{
    public float      SightDistance   = 100.0f;
    public float      AttackDistance  = 50.0f;
    public float      Damping         = 0.9f;
    public GameObject PlayerObject;

    public static string EnemyTag = "Enemy";

    private CharacterController Controller;
    private float               PlayerTargetDistance;
    private ActorController     PlayerActorController;

	void Start ()
    {
        Initialise();
        Controller = GetComponent<CharacterController>();
        if (PlayerObject != null)
            PlayerActorController = PlayerObject.GetComponent<ActorController>();
	}
	
	void FixedUpdate ()
    {
        ActorFixedUpdate();

        if (PlayerActorController != null)
        { 
	        PlayerTargetDistance = Vector3.Distance (PlayerActorController.GetTargetLocation(), transform.position);
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
        if (PlayerActorController != null)
        {
            Vector3 LookVector = PlayerActorController.GetTargetLocation() - transform.position;
            LookVector.y = 0.0f;
            LookRotation = Quaternion.LookRotation (LookVector);
        }
    }

    public void Shoot()
    {
        Shoot (PlayerActorController.GetTargetLocation());
    }
}
