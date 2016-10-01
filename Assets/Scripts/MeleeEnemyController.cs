using UnityEngine;
using System.Collections;

public class MeleeEnemyController : ActorController
{
    public float      SightDistance     = 100.0f;
    public float      FollowDistance    = 50.0f;
    public float      Damping           = 0.9f;
    public float      MeleeAttackDamage = 2.0f;
    public float      BobbingSpeed      = 5.0f;
    public float      BobbingHeight     = 3.0f;
    public GameObject SelfDestructEffect;
    public GameObject PlayerObject;

    public static string EnemyTag = "Enemy";

    private CharacterController Controller;
    private float               PlayerTargetDistance;
    private ActorController     PlayerActorController;
    private bool                HasDetonated = false;

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
            if (Controller != null)
            { 
                MovementVector = Vector3.zero;
                if (PlayerTargetDistance < FollowDistance)
                {
                    UpdateLookRotation();
                    transform.rotation = LookRotation;
                    Vector3 playerTargetPoint = PlayerActorController.GetTargetLocation();
                    MovementVector = (playerTargetPoint - transform.position).normalized; 
                }
                float yAnimation = Mathf.Sin (Time.time * BobbingSpeed) * BobbingHeight * Time.deltaTime;
                MovementVector = new Vector3 (MovementVector.x, MovementVector.y + yAnimation, MovementVector.z);
                Controller.Move ((MovementVector + StaggerMovementVector) * MovementSpeed * Time.deltaTime);
            }
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

    public void SelfDestruct()
    {
        if (SelfDestructEffect != null)
        { 
            Object destructEffect = Instantiate (SelfDestructEffect, transform.position, transform.rotation);
            GameObject destructEffectObject = (GameObject) destructEffect;
            destructEffectObject.GetComponent<ParticleSystem>().Play();
            float duration = destructEffectObject.GetComponent<ParticleSystem>().duration;
            Destroy (destructEffectObject, duration + 0.5f);
            Destroy (gameObject, 0.1f);
        }
    }

    void OnControllerColliderHit (ControllerColliderHit col)
    {
        if (!HasDetonated && col.gameObject.tag == PlayerController.PlayerTag)
        {
            PlayerController player = col.gameObject.GetComponent<PlayerController>();
            if (player != null)
                player.TakeHit (col.point, MeleeAttackDamage, new BulletInfo (Vector3.zero, 1.0f));

            SelfDestruct();
            HasDetonated = true;
        }
    }
}
