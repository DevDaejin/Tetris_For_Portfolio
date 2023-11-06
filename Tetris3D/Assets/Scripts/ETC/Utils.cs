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

    public static Vector3 Vector2IntTo3(Vector2Int vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    public static Vector2Int Vector3To2Int(Vector3 vector)
    {
        return new Vector2Int((int)vector.x, (int)vector.y);
    }
}
