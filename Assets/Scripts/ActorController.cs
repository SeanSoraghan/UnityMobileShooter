﻿using UnityEngine;
using System.Collections;

public class ActorController : MonoBehaviour
{
    public float      StartHealth     = 100.0f;
    public float      MaxHealth       = 100.0f;
    public float      MovementSpeed   = 1000.0f;
    public Vector3    MovementVector  = Vector3.zero;
    public Quaternion LookRotation    = new Quaternion();
    public GameObject ShotImpact;
    public GameObject Gun;

    private float      Health;
    private float      HealthPercentage;


    private GunController GunComponent;

    public void Initialise() { Start(); }
	void Start ()
    {
	    Health = StartHealth;
        UpdateHealthPercentage();

        if (Gun != null)
            GunComponent = Gun.GetComponent<GunController>();
	}
	
    public void CheckDeath()  { if (HealthPercentage <= 0.0f) Object.Destroy (gameObject); }
    public void UpdateActor() { Update(); }
	void Update ()            { CheckDeath(); }

    private void UpdateHealthPercentage()
    {
        HealthPercentage = Health / MaxHealth;
    }

    public void Shoot (Vector3 targetLocation)
    {
        if (GunComponent != null)
            GunComponent.Shoot (targetLocation);
        else
            Debug.Log("null");
    }

    public void TakeHit (Vector3 hitLocation, float damage)
    {
        ShowShotImpact (hitLocation);
        TakeDamage (damage);
    }

    private void TakeDamage (float damage)
    {
        Health -= damage;
        UpdateHealthPercentage();
    }

    private void ShowShotImpact (Vector3 position)
    {
        if (ShotImpact != null)
        { 
            Object gunshot = Instantiate (ShotImpact, position, transform.rotation);
            GameObject gunshotObject = (GameObject) gunshot;
            gunshotObject.GetComponent<ParticleSystem>().Play();
            Destroy (gunshotObject, gunshotObject.GetComponent<ParticleSystem>().duration + 0.5f);
        }
    }

    public void TakeHealth (float Health)
    {
        Health += Health;
        if (Health > MaxHealth)
            Health = MaxHealth;
        UpdateHealthPercentage();
    }

    public void PickUpGun (GameObject newGunObject)
    {
        Destroy (Gun);
        Gun = (GameObject) Instantiate (newGunObject, newGunObject.transform.position, newGunObject.transform.rotation);

        if (Gun != null)
            GunComponent = Gun.GetComponent<GunController>();
        
        Destroy (newGunObject);
    }

    public virtual Vector3 GetTargetLocation()
    {
        return transform.position + new Vector3 (0.0f, transform.localScale.y * 0.5f);
    }
}
