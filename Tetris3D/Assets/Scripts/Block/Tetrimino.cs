using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Tetrimino : Block
{
    
    public TetriminoType GetTetriminoType { get => tetriminoType; }
    public Vector2Int PositionInGrid { get => positionInGrid; set => positionInGrid = value; }

    private ObjectPool<Tetrimino> pool;
    private TetriminoType tetriminoType;
    private Vector2Int positionInGrid;

    private int rotate = 0;
    private int rotateLength = 0;

    private GameObject[,] GameObjectsArray = new GameObject[,] {{null, null, null, null},
                                                                {null, null, null, null},
                                                                {null, null, null, null},
                                                                {null, null, null, null}};

    public void Initialize(ObjectPool<Tetrimino> tetriminoPool)
    {
        pool = tetriminoPool;
        material = new Material(Shader.Find(Constant.LitShaderPath));

        for (int dim1 = 0; dim1 < 4; dim1++)
        {
            for (int dim2 = 0; dim2 < 4; dim2++)
            {
                Vector3 pos = new Vector3(
                        ( dim2 * (Constant.CubeScale + Constant.CubeInterval)),
                        (-dim1 * (Constant.CubeScale + Constant.CubeInterval)), 0);

                var o = Utils.CreateCubeBlock(pos, Constant.CubeScale, parent: transform);
                o.name = $"{dim1 + 1} / {dim2 + 1}";
                o.SetActive(false);
                GameObjectsArray[dim1, dim2] = o;
            }
        }
    }

    public void SetTetrimino()
    {
        rotate = 0;

        var seed = Enum.GetValues(typeof(TetriminoType));
        tetriminoType = (TetriminoType)seed.GetValue(UnityEngine.Random.Range(0, seed.Length));
        UpdateBlock(tetriminoType);
    }

    public void RotateBlock()
    {
        rotate++;

        if (rotate >= rotateLength)
            rotate = 0;

        UpdateBlock(tetriminoType);
    }

    private void UpdateBlock(TetriminoType tetriminoType)
    {
        switch (tetriminoType)
        {
            case TetriminoType.I:
                    UpdateBlock(Color.cyan, TetriminoData.IArray);
                break;
            case TetriminoType.O:
                    UpdateBlock(Color.yellow, TetriminoData.OArray);
                break;
            case TetriminoType.T:
                    UpdateBlock(Constant.Purple, TetriminoData.TArray);
                break;
            case TetriminoType.S:
                    UpdateBlock(Color.green, TetriminoData.SArray);
                break;
            case TetriminoType.Z:
                    UpdateBlock(Color.red, TetriminoData.ZArray);
                break;
            case TetriminoType.L:
                    UpdateBlock(Constant.Orange, TetriminoData.LArray);
                break;
            case TetriminoType.J:
                    UpdateBlock(Color.blue, TetriminoData.JArray);
                break;
        }
    }

    private void UpdateBlock(Color color, bool[][,] allArray)
    {
        blockColor = color;
        array = allArray[rotate];

        rotateLength = allArray.GetLength(0);

        for (int dim1 = 0; dim1 < array.GetLength(0); dim1++)
        {
            for (int dim2 = 0; dim2 < array.GetLength(1); dim2++)
            {
 
                if (GetArray[dim1, dim2])
                {
                    material.SetColor(colorPropertyName, blockColor);
                    GameObjectsArray[dim1, dim2].GetComponent<MeshRenderer>().material = material;
                    GameObjectsArray[dim1, dim2].SetActive(true);
                }
            }
        }

        if (array.GetLength(0) != 4 || array.GetLength(1) != 4)
        {
            for (int dim1 = array.GetLength(0); dim1 < 4; dim1++)
            {
                for (int dim2 = array.GetLength(1); dim2 < 4; dim2++)
                {
                    GameObjectsArray[dim1, dim2].SetActive(false);
                }
            }
        }
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

    public int GetXOffset()
    {
        int offset = 0;
        bool isDone = false;
        for (int col = 0; col < array.GetLength(1); col++)
        {
            for (int row = 0; row < array.GetLength(0); row++)
            {
                if (array[row, col])
                {
                    isDone = true;
                    break;
                }
            }
            if (isDone)
                break;
            else
                offset++;
        }
        return offset;
    }

    public void Release()
    {
        if (pool != null)
        {
            pool.Release(this);            
        }
    }
}
