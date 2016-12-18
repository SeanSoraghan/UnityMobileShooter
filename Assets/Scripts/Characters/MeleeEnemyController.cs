using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AkMeleeAttackTrigger : AkCustomTrigger
{}

public class MeleeEnemyController : ActorController
{
    public float      SightDistance            = 100.0f;
    public float      FollowDistance           = 50.0f;
    public float      Damping                  = 0.9f;
    public float      MeleeAttackDamage        = 2.0f;
    public float      MovementSoundLength      = 16.5f;
    public float      NearbyEnemySpeedIncrease = 2.0f;
    public GameObject PlayerObject;

    public static string EnemyTag = "Enemy";

    private CharacterController Controller;
    private float               PlayerTargetDistance;
    private float               PreviousMovementTriggerTime = 0.0f;
    private float               BaseMovementSpeed;
    private ActorController     PlayerActorController;
    private List<int>           EnemyContactIDs;

	void Start ()
    {
        Initialise();
    }

    public override void Initialise ()
    {
        base.Initialise ();
        Controller = GetComponentInChildren<CharacterController>();

        if (PlayerObject != null)
            PlayerActorController = ObjectUtils.GetActorControllerFromObject (PlayerObject);

        EnemyContactIDs = new List<int>();
        BaseMovementSpeed = MovementSpeed;
    }

    void FixedUpdate ()
    {
        ActorFixedUpdate();
        UpdateMovementSpeed();
        if (PlayerActorController != null)
        { 
	        PlayerTargetDistance = Vector3.Distance (PlayerActorController.GetTargetLocation(), transform.position);
            if (Controller != null)
            { 
                MovementVector = Vector3.zero;
                if (PlayerTargetDistance < FollowDistance)
                {
                    AkMovementTrigger movementTrigger = GetComponent<AkMovementTrigger>();
                    if (movementTrigger != null)
                    {
                        if (Time.time - PreviousMovementTriggerTime > MovementSoundLength)
                        { 
                            movementTrigger.TriggerSound();
                            PreviousMovementTriggerTime = Time.time;
                        }
                    }
                    UpdateLookRotation();
                    transform.rotation = LookRotation;
                    Vector3 playerTargetPoint = PlayerActorController.GetTargetLocation();
                    MovementVector = (playerTargetPoint - transform.position).normalized; 
                }
                Controller.Move ((MovementVector + StaggerMovementVector) * MovementSpeed * Time.deltaTime);
            }
        }
	}

    private void UpdateMovementSpeed()
    {
        float speed = BaseMovementSpeed;

        foreach (int id in EnemyContactIDs)
            speed += NearbyEnemySpeedIncrease;

        MovementSpeed = speed;

        //list will be filled again in the next OnCollisionXXXXX calls
        EnemyContactIDs.Clear();
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

    public virtual void Attack()
    {
        AkMeleeAttackTrigger trigger = GetComponent<AkMeleeAttackTrigger>();
        if (trigger != null)
            trigger.TriggerSound();  
    }

    public void AddEnemyToListOfNeighbours (GameObject gameObject)
    {
        if (! EnemyContactIDs.Contains(gameObject.GetInstanceID()))
            EnemyContactIDs.Add (gameObject.GetInstanceID());
    }
}




