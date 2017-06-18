using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayerObjectMoveInteraction : TriggerActionButtonEnabler
{
    public ObjectMover      Object;

    protected override void UpdateActionButtonCallback(ref PlayerController p)
    {
        if (p != null)
            p.SetPlayerActionCallback(TriggerObjectMove);
    }

    private void TriggerObjectMove()
    {
        if (Object != null)
            Object.TriggerMove(ObjectMover.MoveTriggerType.Player);
    }
}
