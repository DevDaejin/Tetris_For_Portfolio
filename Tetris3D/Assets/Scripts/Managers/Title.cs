using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : Block
{
    private float charInterval = 2;
    private float heightOffset = 1.5f;
    private float textOffset = 1.5f;
    private float animationSpeed = 2;
    private string[] tetris = { "T", "E", "T", "R", "I", "S" };
    private new int[][,] array;
    private Color[] blockColors;
    private Transform blockContainer;
    private TMPro.TextMeshPro tmp;

    private Coroutine titleAnimationCoroutine;

    private readonly string blockContainerName = "Blocks";
    private readonly string pressAnyKey = "Press any key";
    private readonly string textShaderPath = "TextMeshPro/Mobile/Distance Field";

    #region Unity leftcycle
    private void OnEnable()
    {
        titleAnimationCoroutine = StartCoroutine(TitleAnimationCoroutine());
    }

    private void Start()
    {
        blockColors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Constant.Orange, Constant.Purple };

        SetData();
        CreateBlock();
        CreateText();
    }

    private void OnDisable()
    {
        StopCoroutine(titleAnimationCoroutine);
        titleAnimationCoroutine = null;
    }
    #endregion

    IEnumerator TitleAnimationCoroutine()
    {
        yield return new WaitUntil(() => tmp != null);

        tmp.color = Color.white;
        float time = 0;
        while (true)
        {
            tmp.color = new Color(1, 1, 1, (Mathf.Cos(time * animationSpeed) + 1));
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void CreateBlock()
    {
        blockContainer = new GameObject(blockContainerName).transform;
        blockContainer.SetParent(transform);
        blockContainer.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        GameObject titleCharacterBlock;
        Vector3 startPosition = Vector3.left * ((tetris.Length) * 0.5f + charInterval);
        for (int i = 0; i < tetris.Length; i++)
        {
            titleCharacterBlock = new GameObject(tetris[i]);
            titleCharacterBlock.transform.position = startPosition + (Vector3.right * i * charInterval);
            titleCharacterBlock.transform.SetParent(blockContainer);

            float centerX = array[i].GetLength(1) * (Constant.CubeScale + Constant.CubeInterval) * -0.5f;
            float centerY = array[i].GetLength(0) * (Constant.CubeScale + Constant.CubeInterval) * -0.5f;

            for (int row = 0; row < array[i].GetLength(0); row++)
            {
                for (int col = 0; col < array[i].GetLength(1); col++)
                {
                    if (array[i][row, col] == 1)
                    {
                        var o = Utils.CreateCubeBlock(centerX + (col * (Constant.CubeScale + Constant.CubeInterval) + (Constant.CubeScale * 0.5f)),
                                                      -(centerY + (row * (Constant.CubeScale + Constant.CubeInterval))),
                                                      Constant.CubeScale, parent: titleCharacterBlock.transform);

                        var m = o.GetComponent<MeshRenderer>();
                        m.material = new Material(Shader.Find(Constant.LitShaderPath));
                        m.material.color = blockColors[Random.Range(0, blockColors.Length)];
                    }
                }
            }
        }

        transform.position += Vector3.up * heightOffset;
    }

    private void CreateText()
    {
        GameObject text = new GameObject();
        text.transform.SetParent(transform);
        text.transform.position += Vector3.down * textOffset;

        text.AddComponent<MeshRenderer>().material = new Material(Shader.Find(textShaderPath));
        tmp = text.AddComponent<TMPro.TextMeshPro>();
        tmp.text = pressAnyKey;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.fontSize = 7f;
        tmp.color = Color.white;
    }

    private void SetData()
    {
        array = new int[6][,];
        //T
        array[0] = new int[,]
        {
            {1,1,1},
            {0,1,0},
            {0,1,0},
            {0,1,0},
            {0,1,0}
        };
        //E
        array[1] = new int[,]
        {
            {1,1,1},
            {1,0,0},
            {1,1,1},
            {1,0,0},
            {1,1,1},
        };
        //T
        array[2] = new int[,]
         {
            {1,1,1},
            {0,1,0},
            {0,1,0},
            {0,1,0},
            {0,1,0}
         };
        //R
        array[3] = new int[,]
         {
            {1,1,1},
            {1,0,1},
            {1,1,0},
            {1,0,1},
            {1,0,1}
         };
        //I
        array[4] = new int[,]
         {
            {1,1,1},
            {0,1,0},
            {0,1,0},
            {0,1,0},
            {1,1,1}
         };
        //S
        array[5] = new int[,]
         {
            {1,1,1},
            {1,0,0},
            {1,1,1},
            {0,0,1},
            {1,1,1}
         };
    }
}
