using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int[,] GetArray { get => array; }

    protected int[,] array;
    protected Color color = Color.white;
    protected Material material;    
    protected readonly string colorPropertyName = "_BaseColor";
}
