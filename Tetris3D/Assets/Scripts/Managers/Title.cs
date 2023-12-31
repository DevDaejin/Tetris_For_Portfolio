using System.Collections;
using UnityEngine;

public class Title : Block
{
    
    private bool[][,] blockArray;
    private Color[] blockColors;
    private Transform blockContainer;
    private TMPro.TextMeshPro tmp;
    private Coroutine titleAnimationCoroutine;

    private readonly float charInterval = 2;
    private readonly float heightOffset = 1.5f;
    private readonly float textOffset = 1.5f;
    private readonly float animationSpeed = 2;
    private readonly string[] tetris = { "T", "E", "T", "R", "I", "S" };

    private readonly string blockContainerName = "Blocks";
    private readonly string pressAnyKey = "Press the \"space bar\" to get started";
    private readonly string textShaderPath = "TextMeshPro/Mobile/Distance Field";

    private void Awake()
    {
        blockColors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Constant.Orange, Constant.Purple };

        SetData();
        CreateBlock();
        CreateText();
    }
    private void OnEnable()
    {
        titleAnimationCoroutine = StartCoroutine(TitleAnimationCoroutine());
    }

    private void OnDisable()
    {
        StopCoroutine(titleAnimationCoroutine);
        titleAnimationCoroutine = null;
    }

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

            float centerX = blockArray[i].GetLength(1) * (Constant.CubeScale + Constant.CubeInterval) * -0.5f;
            float centerY = blockArray[i].GetLength(0) * (Constant.CubeScale + Constant.CubeInterval) * -0.5f;

            for (int dim1 = 0; dim1 < blockArray[i].GetLength(0); dim1++)
            {
                for (int dim2 = 0; dim2 < blockArray[i].GetLength(1); dim2++)
                {
                    if (blockArray[i][dim1, dim2])
                    {
                        Vector3 pos = new Vector3(
                            centerX + (dim2 * (Constant.CubeScale + Constant.CubeInterval) + (Constant.CubeScale * 0.5f)),
                            -(centerY + (dim1 * (Constant.CubeScale + Constant.CubeInterval))), 0);
                        var o = Utils.CreateCubeBlock(pos, Constant.CubeScale, parent: titleCharacterBlock.transform);

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
        blockArray = new bool[6][,]
        {
            Constant.TArray,
            Constant.EArray,
            Constant.TArray,
            Constant.RArray,
            Constant.IArray,
            Constant.SArray
        };
    }
}
