using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActionButtonEnabler : TriggerPlayerResponder
{
    public ActionButtonController ActionButton;

    protected virtual void UpdateActionButtonCallback(ref PlayerController p) { }

    protected override void OnPlayerEnteredTrigger(ref PlayerController p)
    {
        if (ActionButton != null)
        { 
            ActionButton.gameObject.SetActive(true);
            UpdateActionButtonCallback(ref p);
        }
        p.SetActionEnabled(true);
    }

    protected override void OnPlayerExitedTrigger(ref PlayerController p)
    {
        p.SetActionEnabled(false);
        if (ActionButton != null)
            ActionButton.gameObject.SetActive(false);
    }
}
