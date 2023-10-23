using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int[,] GetArray { get => array; }

    protected int[,] array;
    protected Color color = Color.white;
    protected Material material;
    protected readonly Color purple = new Color(128 / 255, 0, 128 / 255);
    protected readonly Color orange = new Color(1, 127 / 255, 0);
    protected readonly string shaderPath = "Universal Render Pipeline/Lit";
    protected readonly string colorPropertyName = "_BaseColor";
}
