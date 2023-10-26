using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public Vector3 GetSpawnPoint { get => spawnPoint; }
    private Vector3 spawnPoint;

    private float containerZOffset = 0.1f;

    private bool[,] grid;
    private Vector2Int gridSize;
    private float gridScale;
    private float gridInterval;

    private Transform container;
    private readonly string containerName = "Grid";

    public Grid(int row, int col, float gridScale, float gridInterval)
    {
        this.gridScale = gridScale;
        this.gridInterval = gridInterval;

        gridSize = new Vector2Int(row, col);
        grid = new bool[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y] = false;
            }
        }        

        GenerateGrid();
    }

    public void GenerateGrid()
    {
        container = new GameObject(containerName).transform;
        container.SetPositionAndRotation(Vector3.forward * containerZOffset, Quaternion.identity);

        float positionX;
        float positionY;

        float xHalf = gridSize.x * 0.5f;
        float yHalf = gridSize.y * 0.5f;

        GameObject o;

        for (float y = 0; y < gridSize.y; y++)
        {
            for (float x = 0; x < gridSize.x; x++)
            {
                positionX = CalcuratePosition(x, gridScale, xHalf, gridInterval);
                positionY = -CalcuratePosition(y, gridScale, yHalf, gridInterval);
                Vector3 pos = new Vector3(positionX, positionY, 0);
                o = Utils.CreateCubeBlock(pos, gridScale, $"{x + 1} {y + 1} tile", container);
                var m = o.GetComponent<MeshRenderer>();
                m.material = new Material(Shader.Find(Constant.LitShaderPath));
                m.material.color = Color.gray;

                if (x == Constant.SpawnRow && y == 0)
                    spawnPoint = o.transform.localPosition;
            }
        }

        ActiveGrid(false);
    }

    public void ActiveGrid(bool isActive = true)
    {
        container.gameObject.SetActive(isActive);
    }

    private float CalcuratePosition(float index, float scale, float halfOfMaxSize, float offset)
    {
        //계산 된 포지션 - 센터 맞추기 위한 값
        //Scale 값 반영 x + Cube간 이격 - (전체 크기의 반 + 이격 거리로 인해 벌어진값 반 + Cube Size의 반)
        return (index * scale) + (index * offset) - ((halfOfMaxSize * scale) + (halfOfMaxSize * offset) - ((scale + offset) * 0.5f));
    }

    public bool GetGrid(int dim1, int dim2)
    {
        return grid[dim1, dim2];
    }

    public void SetGrid(int dim1, int dim2, bool value)
    {
        grid[dim1, dim2] = value;
    }

    public bool IsMovingValidation(Tetrimino tetrimino, Vector2Int moveVector)
    {
        if(IsInTheGrid(tetrimino, moveVector) /*&& !IsCollision(tetrimino)*/)
        {
            tetrimino.PositionInGrid += moveVector;
            return true;
        }

        return false;
    }

    private bool IsInTheGrid(Tetrimino tetrimino, Vector2Int moveVector)
    {
        var newPosition = tetrimino.PositionInGrid + moveVector;
        if (newPosition.x + tetrimino.GetWidth() < gridSize.x && newPosition.x >= 0 && newPosition.y > -gridSize.y)
        {
            return true;
        }

        return false;
    }

    private bool IsCollision(Tetrimino tetrimino)
    {
        var x = tetrimino.PositionInGrid.x;
        var y = Mathf.Abs(tetrimino.PositionInGrid.y);

        for (int i = 0; i < tetrimino.GetArray.GetLength(0); i++)
        {
            for (int j = 0; j < tetrimino.GetArray.GetLength(1); j++)
            {
                if (x + i < gridSize.x && x + i >= 0 && y + j < gridSize.y && y + j >= 0)
                {
                    if (grid[x+i, y+j] && tetrimino.GetArray[i, j])
                        return true;
                }
            }
        }

        if (y + tetrimino.GetHeight() >= gridSize.y)
            return true;

        return false;
    }

    private bool CheckFullLine(int dim1)
    {
        for (int dim2 = 0; dim2 < gridSize.y ; dim2++)
        {
            if (!grid[dim1, dim2])
                return false;
        }

        return true;
    }

    public void DebugGrid()
    {
        string s = string.Empty;

        for (int dim2 = 0; dim2 < grid.GetLength(1); dim2++)
        {
            s += "\n";
            for (int dim1 = 0; dim1 < grid.GetLength(0); dim1++)
            {
                s += grid[dim1, dim2] ? "O" : "X";
            }
        }

        Debug.Log(s);
    }
}
