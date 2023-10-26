using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool[,] GetArray { get => array; }
    protected bool[,] array;
    protected Color color = Color.white;
    protected Material material;
    protected readonly string colorPropertyName = "_BaseColor";
}
