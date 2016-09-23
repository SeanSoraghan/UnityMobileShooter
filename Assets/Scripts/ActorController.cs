using UnityEngine;
using System.Collections;

public class ActorController : MonoBehaviour
{
    public float      StartHealth = 100.0f;
    public float      MaxHealth   = 100.0f;
    public GameObject ShotImpact;
    
    private float Health;
    private float HealthPercentage;

	void Start ()
    {
	    Health = StartHealth;
        UpdateHealthPercentage();
	}
	
	void Update ()
    {
	    if (HealthPercentage <= 0.0f)
            Object.Destroy (gameObject);
	}

    private void UpdateHealthPercentage()
    {
        HealthPercentage = Health / MaxHealth;
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
}
