using UnityEngine;
using System.Collections;

public class DoorCheckerScript : MonoBehaviour
{
    public GameObject[] Enemies;
    public GameObject   Door;

    private DoorScript DoorController;
    private bool DoorOpened = false;

	void Start ()
    {
	    if (Door != null)
            DoorController = Door.GetComponent<DoorScript>();
	}
	
	void Update ()
    {
	    if (!DoorOpened && AreAllEnemiesDestroyed() && DoorController != null)
        { 
            DoorController.Open();
            DoorOpened = true;
        }
	}

    bool AreAllEnemiesDestroyed()
    {
        foreach (GameObject enemy in Enemies)
            if (enemy.gameObject != null)
                return false;

        return true;
    }
}
