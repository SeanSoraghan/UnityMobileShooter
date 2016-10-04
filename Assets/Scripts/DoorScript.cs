using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public bool IsOpening = false;
    public Transform TargetPosition;
    public float OpenSpeed = 0.01f;

    private Vector3 StartPosition     = Vector3.zero;
    private Vector3 EndPosition       = Vector3.zero;
    private float   interpolationTime = 0.0f;

	void Start ()
    {
	
	}
	
	void Update ()
    {
	    if (IsOpening)
        {
            float t = interpolationTime * interpolationTime;
            transform.position = Vector3.Lerp (StartPosition, EndPosition, t);
            float eps = 0.00001f;

            interpolationTime += OpenSpeed;

            if (Vector3.Distance (transform.position, EndPosition) < eps)
                IsOpening = false;
        }
	}

    public void Open()
    {
        if (TargetPosition != null)
        {
            StartPosition = transform.position;
            EndPosition   = TargetPosition.position;
            IsOpening = true;
        }
    }
}
