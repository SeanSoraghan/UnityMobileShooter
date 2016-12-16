using UnityEngine;
using System.Collections;

public class EnvironmentConditionChecker : MonoBehaviour
{
    public bool IsConditionMet() { return ConditionMet; }

    private bool ConditionMet = false;

	void Start ()
    {}
	
	void Update ()
    {
        ConditionMet = CheckCondition();
    }

    public virtual bool CheckCondition() { return false; }
}
