using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino : Block
{
    public TetriminoType GetTetriminoType { get => tetriminoType; }
    [SerializeField] private TetriminoType tetriminoType;
    public Vector2Int PositionInGrid { get; set; }

    private int rotate = 0;
    private int rotateLength = 0;
    private bool isInitialized = false;
    private bool isInit = false;
    private GameObject[,] GameObjectsArray = new GameObject[,] {{null, null, null, null},
                                                                {null, null, null, null},
                                                                {null, null, null, null},
                                                                {null, null, null, null}};

    public void SetTetrimino()
    {
        if (!isInit)
        {
            material = new Material(Shader.Find(Constant.LitShaderPath));
            isInit = true;
        }

        var seed = Enum.GetValues(typeof(TetriminoType));
        tetriminoType = (TetriminoType)seed.GetValue(UnityEngine.Random.Range(0, seed.Length));

        switch (tetriminoType)
        {
            case TetriminoType.I:
                {
                    UpdateBlock(Color.cyan, TetriminoData.IArray[0]);
                }
                break;
            case TetriminoType.O:
                {
                    UpdateBlock(Color.yellow, TetriminoData.OArray[0]);
                }
                break;
            case TetriminoType.T:
                {
                    UpdateBlock(Constant.Purple, TetriminoData.TArray[0]);
                }
                break;
            case TetriminoType.S:
                {
                    UpdateBlock(Color.green, TetriminoData.SArray[0]);
                }
                break;
            case TetriminoType.Z:
                {
                    UpdateBlock(Color.red, TetriminoData.ZArray[0]);
                }
                break;
            case TetriminoType.L:
                {
                    UpdateBlock(Constant.Orange, TetriminoData.LArray[0]);
                }
                break;
            case TetriminoType.J:
                {
                    UpdateBlock(Color.blue, TetriminoData.JArray[0]);
                }
                break;
        }

        PositionInGrid = new Vector2Int(Constant.SpawnRow, -GetHeight());
    }

    private void RotateBlock()
    {

    }

    private void UpdateBlock(Color color, bool[,] array)
    {
        this.color = color;
        this.array = array;

        for (int dim1 = 0; dim1 < GetArray.GetLength(0); dim1++)
        {
            for (int dim2 = 0; dim2 < GetArray.GetLength(1); dim2++)
            {
                if (!isInitialized)
                {
                    rotateLength = array.GetLength(0);
                    Debug.Log(rotateLength);
                    Vector3 pos = new Vector3(
                        (dim2 * (Constant.CubeScale + Constant.CubeInterval)),
                        (-dim1 * (Constant.CubeScale + Constant.CubeInterval)), 0);

                    var o = Utils.CreateCubeBlock(pos, Constant.CubeScale, parent: transform);

                    o.name = $"{dim1 + 1} / {dim2 + 1}";
                    o.gameObject.SetActive(false);
                    GameObjectsArray[dim1, dim2] = o;
                }

                if (GetArray[dim1, dim2])
                {
                    material.SetColor(colorPropertyName, color);
                    GameObjectsArray[dim1, dim2].GetComponent<MeshRenderer>().material = material;
                    GameObjectsArray[dim1, dim2].SetActive(true);
                }
                else
                {
                    GameObjectsArray[dim1, dim2].SetActive(false);
                }

            }
        }

        if (!isInitialized)
            isInitialized = true;
    }

    public int GetHeight()
    {
        int height = 0;
        for (int row = 0; row < array.GetLength(0); row++)
        {
            for (int col = 0; col < array.GetLength(1); col++)
            {
                if (array[row, col] && height < row)
                    height = row;
            }
        }
        return height;
    }

    public int GetWidth()
    {
        int width = 0;
        for (int row = 0; row < array.GetLength(0); row++)
        {
            for (int col = 0; col < array.GetLength(1); col++)
            {
                if (array[row, col] && width < col)
                    width = col;
            }
        }
        return width;
    }
}
