using UnityEngine;
using System.Collections;

public class AkAmbientTrigger : AkCustomTrigger { }

public class AmbientLoopTrigger : MonoBehaviour
{
    public float TriggerProbability  = 0.5f;
    public float TimeBetweenTriggers = 1.0f;

    private float PreviousTriggerTime = 0.0f;
     
	void Start ()
    {}
	
	void Update ()
    {
	    if (Time.time - PreviousTriggerTime > TimeBetweenTriggers)
        {
            AkAmbientTrigger trigger = GetComponent<AkAmbientTrigger>();
            if (trigger != null && Random.Range (0.0f, 1.0f) < TriggerProbability)
                trigger.TriggerSound();
            PreviousTriggerTime = Time.time;
        }
	}
}
