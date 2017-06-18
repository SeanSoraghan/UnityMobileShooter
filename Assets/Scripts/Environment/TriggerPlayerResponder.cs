using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayerResponder : MonoBehaviour
{
    protected virtual void OnPlayerEnteredTrigger(ref PlayerController Player) { }
    protected virtual void OnPlayerExitedTrigger(ref PlayerController Player)  { }

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.tag == PlayerController.PlayerTag)
        { 
            PlayerController p = other.gameObject.GetComponent<PlayerController>();
            if (p != null)
                OnPlayerEnteredTrigger(ref p);
        }
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.gameObject.tag == PlayerController.PlayerTag)
        {
            PlayerController p = other.gameObject.GetComponent<PlayerController>();
            if (p != null)
                OnPlayerExitedTrigger(ref p);
        }
    }
}
