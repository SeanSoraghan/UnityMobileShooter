using UnityEngine;
using System.Collections;

public class ObjectUtils {
   
	public static ActorController GetActorControllerFromObject (GameObject obj)
    {
        ActorController actor = obj.GetComponentInChildren<ActorController>();
        if (actor == null)
            actor = obj.GetComponentInParent<ActorController>();
        return actor;
    }

    public static EnemyController GetEnemyControllerFromObject (GameObject obj)
    {
        EnemyController actor = obj.GetComponentInChildren<EnemyController>();
        if (actor == null)
            actor = obj.GetComponentInParent<EnemyController>();
        return actor;
    }
}
