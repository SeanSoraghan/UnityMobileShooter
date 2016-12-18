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
}
