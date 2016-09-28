using UnityEngine;
using System.Collections;

public class TestAnimatorControllerScript : MonoBehaviour {

	// Use this for initialization
    private bool overrideTest = false;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButtonDown(0))
        {
            overrideTest = !overrideTest;
            GetComponent<Animator>().SetBool ("overrideTest", overrideTest);
            Debug.Log("test: " + overrideTest);
        }
	}
}
