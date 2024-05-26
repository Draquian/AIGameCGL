using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinManager : MonoBehaviour
{
    GameObject target = null;

    public void SetTarget(GameObject t)
    {
        target = t;
    }

    public GameObject GetTarget()
    {
        return target;
    }
}
