using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailAnimator : MonoBehaviour
{
    [Range(0.0f, 20.0f)]
    public float WaveSpeed      = 5.0f;
    [Range(0.0f, 360.0f)]
    public float WaveAmpDegrees = 40.0f;
    [Range(-1.0f, 1.0f)]
    public float WaveOffset     = -0.3f;
    List<GameObject> Segments;

    public static Transform GetChildSegment(Transform t)
    {
        int NumChildren = t.childCount;
        for (int c = 0; c < NumChildren; ++c)
            if (t.GetChild(c).gameObject.transform.childCount > 0)
                return t.GetChild(c);
        return null;
    }

	// Use this for initialization
	void Start ()
    {
        Segments = new List<GameObject>();
		Transform childTransform = GetChildSegment(transform);
        while (childTransform != null)
        {
            Segments.Add(childTransform.gameObject);
            childTransform = GetChildSegment(childTransform);
        }
	}

	// Update is called once per frame
	void FixedUpdate ()
    {
        float angle = Time.time * WaveSpeed;
        float offset = 0.0f;
		int NumSegments = Segments.Count;
        for (int s = 0; s < NumSegments; ++s)
        {
            Vector3 r = Segments[s].transform.rotation.eulerAngles;
            float si = Mathf.Sin(angle + offset);
            Segments[s].transform.rotation = Quaternion.Euler (si * WaveAmpDegrees, r.y, r.z);
            offset += WaveOffset * Mathf.PI;
        }
	}
}
