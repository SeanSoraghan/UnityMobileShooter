using UnityEngine;
using System.Collections;

public struct BulletInfo
{
    public BulletInfo (Vector3 velocty, float damageMultiplier)
    {
        Velocity = velocty;
        DamageMultiplier = damageMultiplier;
    }

    public Vector3 Velocity;
    public float   DamageMultiplier;
}

public class GunController : MonoBehaviour
{
    public float         ShotDamage               = 10.0f;
    public float         FireRateSeconds          = 2.0f;
    public GameObject    GunshotEffect;
    public Transform     GunEnd;
    public bool          IsPickup                 = false;
    public float         PickupAnimationSpeed     = 1.0f;
    public float         PickupAnimationBobHeight = 0.2f;
    public float         PickupAnimationBobSpeed  = 10.0f;
    public static string GunTag                   = "Gun";

    private float      LastShotTime            = 0.0f;
    private float      SecondsBetweenShots     = 1.0f;
    private RaycastHit shootRaycastResult;
    private float      PickUpAnimationRotation = 0.0f;
    private Vector3    DefaultPosition;
    private Vector3    DefaultScale;
    private Quaternion DefaultRotation;

	void Start ()
    {
        LastShotTime        = 0.0f;
        CalculateFireRate();
        DefaultPosition     = transform.localPosition;
        DefaultScale        = transform.localScale;
        DefaultRotation     = transform.localRotation;
	}
	
    public void CalculateFireRate()
    {
	    SecondsBetweenShots = 1.0f / FireRateSeconds;
    }

    public void MakeReal()
    {
        IsPickup = false;
        transform.localRotation = DefaultRotation;
        transform.localScale    = DefaultScale;
        transform.localPosition = DefaultPosition;
    }

	void Update ()
    {
        if (IsPickup)
        {
            Vector3 r = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler (r.x, PickUpAnimationRotation, r.z);

            Vector3 p = transform.localPosition;
            float animationBob = Mathf.Sin (Time.time * PickupAnimationBobSpeed) * PickupAnimationBobHeight;
            transform.localPosition = new Vector3 (p.x, p.y + animationBob, p.z);

            PickUpAnimationRotation += PickupAnimationSpeed;
        }
    }

    public void Shoot (Vector3 TargetLocation)
    {
        if (Time.time > LastShotTime + SecondsBetweenShots)
        {
            Vector3 shootRayOrigin = GetShootOrigin();
            Vector3 shootRayDirection = (TargetLocation - shootRayOrigin).normalized;
            if (Physics.Raycast (shootRayOrigin, shootRayDirection, out shootRaycastResult))
            {
                Vector3 debugRayLength = shootRaycastResult.point - shootRayOrigin;
                Debug.DrawRay (shootRayOrigin, debugRayLength, new Color (1.0f, 0.0f, 0.0f), 0.2f, true);

                AkCustomTrigger gunshotSound = GetComponent<AkCustomTrigger>();

                if (gunshotSound != null)
                    gunshotSound.TriggerSound();

                if (GunshotEffect != null)
                { 
                    Object gunshot = Instantiate (GunshotEffect, shootRayOrigin, transform.rotation);
                    GameObject gunshotObject = (GameObject) gunshot;
                    gunshotObject.transform.parent        = GunEnd;
                    gunshotObject.transform.localRotation = Quaternion.identity;
                    gunshotObject.transform.localPosition = Vector3.zero;
                    gunshotObject.transform.localScale    = Vector3.one;
                    gunshotObject.GetComponent<ParticleSystem>().Play();
                    Destroy (gunshotObject, gunshotObject.GetComponent<ParticleSystem>().duration + 0.5f);
                }

                GameObject objectHit = shootRaycastResult.collider.gameObject;
                ActorController actor = ObjectUtils.GetActorControllerFromObject (objectHit);

                if (actor != null)
                    actor.TakeHit (shootRaycastResult.point, ShotDamage, new BulletInfo (shootRayDirection, 1.0f));
            }
            LastShotTime = Time.time;
        }
    }

    public virtual Vector3 GetShootOrigin()
    {
        if (GunEnd != null)
            return GunEnd.position;
        else
            return transform.position + new Vector3 (transform.localScale.x / 2.0f, 0.0f, 0.0f);
    }
}
