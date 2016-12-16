using UnityEngine;
using System.Collections;

public class SpotlightPickupScript : MonoBehaviour
{
    public Light      Spotlight;
    public GameObject PickupObject;

    private bool LightDestroyed = false;

	// Use this for initialization
	void Start ()
    {
	    Spotlight = GetComponentInChildren<Light>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (!LightDestroyed && PickupObject.gameObject == null)
        { 
            LightDestroyed = true;
            Destroy (Spotlight.gameObject);
            Destroy (this.gameObject);
        }
	}
}
