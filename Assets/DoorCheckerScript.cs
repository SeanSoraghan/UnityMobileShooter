using UnityEngine;
using System.Collections;

public class DoorCheckerScript : MonoBehaviour
{
    public GameObject Enemy;
    public GameObject Door;

    private DoorScript DoorController;
    private bool DoorOpened = false;

	void Start ()
    {
	    if (Door != null)
            DoorController = Door.GetComponent<DoorScript>();
	}
	
	void Update ()
    {
	    if (!DoorOpened && Enemy.gameObject == null)
        { 
            DoorController.Open();
            DoorOpened = true;
        }
	}
}
