using UnityEngine;
using System.Collections;

public class ActorController : MonoBehaviour
{
    public float      StartHealth           = 100.0f;
    public float      MaxHealth             = 100.0f;
    public float      MovementSpeed         = 1000.0f;
    public Vector3    MovementVector        = Vector3.zero;
    public Vector3    StaggerMovementVector = Vector3.zero;
    public Quaternion LookRotation          = new Quaternion();
    public GameObject ShotImpact;
    public GameObject Gun;
    public Transform  TargetPosition;
    public bool       IsAlive               = true;
    public float      StaggerResistance     = 5.0f;

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
	
    public void CheckDeath()
    {
        if (HealthPercentage <= 0.0f)
        {
            IsAlive = false;
            Destroy (gameObject);
        }
    }

    public void UpdateActor() { Update(); }
	void Update ()            { CheckDeath(); }

    public void ActorFixedUpdate() { FixedUpdate(); }
    void FixedUpdate()
    {
        StaggerMovementVector = Vector3.Lerp (StaggerMovementVector, Vector3.zero, Time.deltaTime * (1.0f + StaggerResistance));
    }

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

    public virtual void TakeHit (Vector3 hitLocation, float damage, BulletInfo bulletInfo)
    {
        ShowShotImpact (hitLocation);
        TakeDamage (damage * bulletInfo.DamageMultiplier);
        TakeImpact (bulletInfo);
    }

    public virtual void TakeDamage (float damage)
    {
        Health -= damage;
        UpdateHealthPercentage();
        CheckDeath();
    }

    public virtual void TakeImpact (BulletInfo bulletInfo) { StaggerMovementVector += bulletInfo.Velocity; }

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
        if (TargetPosition != null)
            return TargetPosition.position;

        return transform.position + new Vector3 (0.0f, transform.localScale.y * 0.5f);
    }
}
