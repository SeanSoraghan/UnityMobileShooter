using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public GameObject PlayerObject;
    public float heightAbovePlayer = 10.0f;

	void Start ()
    {}
	
	void Update ()
    {
	    UpdateCameraPosition();
	}

    private void UpdateCameraPosition()
    {
        Transform playerTransform = PlayerObject.transform;
        transform.position = playerTransform.position + new Vector3(0.0f, heightAbovePlayer, 0.0f);
    }
}
