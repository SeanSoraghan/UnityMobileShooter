using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarMover : ObjectMover
{
    public PillarMover VerticalNeighbour;
    public PillarMover DiagonalNeighbour;

    public override void TriggerMove (MoveTriggerType t)
    {
        base.TriggerMove (t);
        if (DiagonalNeighbour != null)
            if (IsTransitioningToPositionStateByTriggerType(PositionState.Original, MoveTriggerType.Player))
                if (DiagonalNeighbour.IsInOrChangingToPositionState(PositionState.Original))
                    DiagonalNeighbour.TriggerMove(MoveTriggerType.Environment);
        if (VerticalNeighbour != null)
            if (IsTransitioningToPositionStateByTriggerType(PositionState.Transformed, MoveTriggerType.Player))
                if (VerticalNeighbour.IsInOrChangingToPositionState(PositionState.Transformed))
                    VerticalNeighbour.TriggerMove(MoveTriggerType.Environment);
    }
}
