using UnityEngine;
using System.Collections;

public class ObjectUtils
{
   
	public static ActorController GetActorControllerFromObject (GameObject obj)
    {
        ActorController actor = obj.GetComponentInChildren<ActorController>();
        if (actor == null)
            actor = obj.GetComponentInParent<ActorController>();
        return actor;
    }

    public static MeleeEnemyController GetMeleeEnemyControllerFromObject (GameObject obj)
    {
        MeleeEnemyController actor = obj.GetComponentInChildren<MeleeEnemyController>();
        if (actor == null)
            actor = obj.GetComponentInParent<MeleeEnemyController>();
        return actor;
    }
}
