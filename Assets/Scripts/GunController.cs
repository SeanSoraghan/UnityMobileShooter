using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    public float      ShotDamage      = 10.0f;
    public float      FireRateSeconds = 2.0f;
    public GameObject GunshotEffect;
    public Transform  GunEnd;

    public static string GunTag = "Gun";

    private float      LastShotTime        = 0.0f;
    private float      SecondsBetweenShots = 1.0f;
    private RaycastHit shootRaycastResult;

	void Start ()
    {
	    SecondsBetweenShots = 1.0f / FireRateSeconds;
	}
	
	void Update ()
    {
	
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

                if (GunshotEffect != null)
                { 
                    Object gunshot = Instantiate (GunshotEffect, shootRayOrigin, transform.rotation);
                    GameObject gunshotObject = (GameObject) gunshot;
                    gunshotObject.GetComponent<ParticleSystem>().Play();
                    Destroy (gunshotObject, gunshotObject.GetComponent<ParticleSystem>().duration + 0.5f);
                }

                GameObject objectHit = shootRaycastResult.collider.gameObject;
                ActorController actor = objectHit.GetComponent<ActorController>();
                if (actor != null)
                    actor.TakeHit (shootRaycastResult.point, ShotDamage);
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
