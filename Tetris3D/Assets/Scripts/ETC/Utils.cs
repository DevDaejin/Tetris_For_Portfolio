using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static GameObject CreateCubeBlock(Vector3 localPosition, float scaleRatio, string name = "Block", Transform parent = null)
    {
        var o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        o.name = name;
        o.transform.SetParent(parent);
        o.transform.localScale = Vector3.one * scaleRatio;
        o.transform.localPosition = localPosition;
        return o;
    }

    public static  Vector3 MoveScale(Vector3 direction)
    {
        return direction * (Constant.CubeInterval + Constant.CubeScale);
    }
}
