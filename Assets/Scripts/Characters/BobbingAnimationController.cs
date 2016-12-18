using UnityEngine;
using System.Collections;

public class BobbingAnimationController : MonoBehaviour
{
    public float BobbingSpeed  = 5.0f;
    public float BobbingHeight = 3.0f;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    float yAnimation = Mathf.Sin (Time.time * BobbingSpeed) * BobbingHeight * Time.deltaTime;
        Vector3 p = transform.localPosition;
        transform.localPosition = new Vector3 (p.x, p.y + yAnimation, p.z);
	}
}
