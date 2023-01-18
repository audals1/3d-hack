using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static Transform FindChildObject(GameObject parent, string ChildName)
    {
        var childObjects = parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childObjects.Length; i++)
        {
            if(childObjects[i].name.Equals(ChildName))
            {
                return childObjects[i].transform;
            }
        }
        return null;
    }
}
