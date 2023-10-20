using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static GameObject CreateCubeBlock(float x, float y, float scaleRatio, string name = "Block", Transform parent = null)
    {
        var o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        o.name = name;
        o.transform.SetParent(parent);
        o.transform.localScale = Vector3.one * scaleRatio;
        o.transform.position = new Vector3(x, y, 0);
        return o;
    }
}
