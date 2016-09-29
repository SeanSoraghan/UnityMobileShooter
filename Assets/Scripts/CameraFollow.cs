using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform PlayerTransform;

    private Vector3 Offset = Vector3.zero;

	void Start ()
    {
        if (PlayerTransform != null)
            Offset = transform.position - PlayerTransform.position;
    }
	
	void Update ()
    {
	    UpdateCameraPosition();
	}

    private void UpdateCameraPosition()
    {
        if (PlayerTransform != null)
            transform.position = PlayerTransform.position + Offset;
    }
}
