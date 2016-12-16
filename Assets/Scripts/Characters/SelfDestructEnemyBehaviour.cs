using UnityEngine;
using System.Collections;

public class SelfDestructEnemyBehaviour : MeleeEnemyController
{
    public GameObject SelfDestructEffect;

    private bool HasDetonated = false;

    void Start()
    {
        Initialise();
    }

    public override void Die()
    {
        Attack();   
    }

    public override void Attack ()
    {
        base.Attack ();
        if (SelfDestructEffect != null)
        { 
            Object destructEffect = Instantiate (SelfDestructEffect, transform.position, transform.rotation);
            GameObject destructEffectObject = (GameObject) destructEffect;
            destructEffectObject.GetComponent<ParticleSystem>().Play();
            float duration = destructEffectObject.GetComponent<ParticleSystem>().duration;
            Destroy (destructEffectObject, duration + 0.5f);

            if (ParentSpawner != null)
                ParentSpawner.DecrementNumLiveSpawns();
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

            Attack();
            HasDetonated = true;
        }
        if (col.gameObject.tag == EnemyController.EnemyTag)
            AddEnemyToListOfNeighbours (col.gameObject);
    }

}
