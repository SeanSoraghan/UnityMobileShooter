using UnityEngine;
using System.Collections;

public class EnemyDeathCheckerScript : MonoBehaviour
{
    public GameObject[] Enemies;
    public GameObject   MovableObject;

    private ObjectMover MoverScript;
    private bool MoveObjectTriggered = false;

	void Start ()
    {
	    if (MovableObject != null)
            MoverScript = MovableObject.GetComponent<ObjectMover>();
	}
	
	void Update ()
    {
	    if (!MoveObjectTriggered && AreAllEnemiesDestroyed() && MoverScript != null)
        { 
            MoverScript.TriggerMove(ObjectMover.MoveTriggerType.Environment);
            MoveObjectTriggered = true;
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
