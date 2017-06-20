using UnityEngine;
using System.Collections;

public class MeleeAttackEnemyBehaviour : MeleeEnemyController
{
    public GameObject AttackEffect;
    public float AttackRateSeconds = 1.0f;

    private float LastAttackTime        = 0.0f;
    private float SecondsBetweenAttacks = 1.0f;
    

    void Start()
    {
        Initialise();
        SecondsBetweenAttacks = 1.0f / AttackRateSeconds;
    }

    public override void Attack ()
    {
        base.Attack ();
        if (AttackEffect != null)
        { 
            Object attackEffect = Instantiate (AttackEffect, transform.position, transform.rotation);
            GameObject attackEffectObject = (GameObject) attackEffect;
            attackEffectObject.GetComponent<ParticleSystem>().Play();
            float duration = attackEffectObject.GetComponent<ParticleSystem>().main.duration;
            Destroy (attackEffectObject, duration + 0.5f);
        }
    }

    void OnControllerColliderHit (ControllerColliderHit col)
    {
        if (Time.time - LastAttackTime > SecondsBetweenAttacks && col.gameObject.tag == PlayerController.PlayerTag)
        {
            PlayerController player = col.gameObject.GetComponent<PlayerController>();
            if (player != null)
                player.TakeHit (col.point, MeleeAttackDamage, new BulletInfo (Vector3.zero, 1.0f));

            Attack();
            LastAttackTime = Time.time;
        }
        if (col.gameObject.tag == EnemyController.EnemyTag)
            AddEnemyToListOfNeighbours (col.gameObject);
    }
}
