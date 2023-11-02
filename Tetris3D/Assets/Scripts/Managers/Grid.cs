using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid
{
    public Vector3 GetSpawnPoint { get => spawnPoint; }
    private Vector3 spawnPoint;

    private List<int> toBeRemoveLineList = new List<int>();

    private float containerZOffset = 0.1f;

    private bool[,] grid;
    private Vector2Int gridSize;
    private float gridScale;
    private float gridInterval;

    public int RowLength { private set; get; }
    public int ColLength { private set; get; }

    private Shader gridShader;
    private Transform container;

    private readonly string containerName = "Grid";
    private readonly string empty = " ○ ";
    private readonly string full = " ● ";

    public Grid(Vector2Int gridSize, float gridScale, float gridInterval)
    {
        this.gridScale = gridScale;
        this.gridInterval = gridInterval;
        this.gridSize = new Vector2Int(gridSize.x, gridSize.y);

        RowLength = gridSize.x;
        ColLength = gridSize.y;

        gridShader = Shader.Find(Constant.LitShaderPath);
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        container = new GameObject(containerName).transform;
        container.SetPositionAndRotation(Vector3.forward * containerZOffset, Quaternion.identity);

        grid = new bool[gridSize.x, gridSize.y];

        float gridWidthHalf = gridSize.x * 0.5f;
        float gridHeightHalf = gridSize.y * 0.5f;

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                grid[x, y] = false;

                float positionX = CalcuratePosition(x, gridWidthHalf);
                float positionY = -CalcuratePosition(y, gridHeightHalf);
                var pos = new Vector3(positionX, positionY, 0);
                var o = Utils.CreateCubeBlock(pos, gridScale, $"{x + 1} {y + 1}", container);

                var m = o.GetComponent<MeshRenderer>();
                m.material = new Material(gridShader);
                m.material.color = Color.gray;

                if (x == Constant.SpawnRow && y == 0)
                    spawnPoint = o.transform.localPosition;
            }
        }

        ActiveGrid(false);
    }

    public void InitGrid()
    {
        grid = new bool[gridSize.x, gridSize.y];
        container.GetComponentsInChildren<MeshRenderer>().ToList().ForEach((mr) => mr.material.color = Color.gray);
    }

    public void ActiveGrid(bool isActive = true)
    {
        container.gameObject.SetActive(isActive);
    }

    private float CalcuratePosition(float index, float halfOfMaxSize)
    {
        //계산 된 포지션 - 센터 맞추기 위한 값
        //Scale 값 반영 x + Cube간 이격 - (전체 크기의 반 + 이격 거리로 인해 벌어진값 반 + Cube Size의 반)
        return (index * gridScale) + (index * gridInterval) - ((halfOfMaxSize * gridScale) + (halfOfMaxSize * gridInterval) - ((gridScale + gridInterval) * 0.5f));
    }

    public Vector2Int WallKick(Tetrimino tetrimino)
    {
        var offset = Vector2Int.zero;
        var PositionInGrid = tetrimino.PositionInGrid;
        var BoundaryDict = tetrimino.BoundaryDict;


        if (PositionInGrid.x + BoundaryDict[Boundary.Left] < 0)
        {
            offset += Vector2Int.left * (PositionInGrid.x + BoundaryDict[Boundary.Left]);
        }

        if (PositionInGrid.x + BoundaryDict[Boundary.Right] > Constant.GridSize.x)
        {
            offset += Vector2Int.right * (Constant.GridSize.x - (PositionInGrid.x + BoundaryDict[Boundary.Right]));
        }

        if (Mathf.Abs(PositionInGrid.y) + BoundaryDict[Boundary.Bottom] > Constant.GridSize.y)
        {
            offset += Vector2Int.up * ((Mathf.Abs(PositionInGrid.y) + BoundaryDict[Boundary.Bottom]) - Constant.GridSize.y);
        }

        return offset;
    }
    public bool IsRotateValidation(bool[,] array, Vector2Int positionInGrid)
    {
        for (int row = 0; row < array.GetLength(0); row++)
        {
            for (int col = 0; col < array.GetLength(1); col++)
            {
                var x = row + positionInGrid.x;
                var y = col + Mathf.Abs(positionInGrid.y);
                if (x > 0 && x < gridSize.x && y > 0 && y < gridSize.y)
                {
                    if (grid[x, y] && array[row, col])
                        return false;
                }
            }
        }

        return true;
    }

    public int CheckHighestBlock(Tetrimino tetrimino)
    {
        var boundary = tetrimino.BoundaryDict;
        var posInGrid = tetrimino.PositionInGrid;

        int left = posInGrid.x + boundary[Boundary.Left];
        int right = posInGrid.x + boundary[Boundary.Right];
        int top = Mathf.Abs(posInGrid.y);
        int bottom = top + boundary[Boundary.Bottom];

        int highest = ColLength;

        for (int row = left; row < right; row++)
        {
            for (int col = top; col < ColLength; col++)
            {
                if (grid[row, col] && highest > col)
                {
                    highest = col;
                }
            }
        }

        return highest;
    }

    public bool IsMovingValidation(Tetrimino tetrimino, Vector2Int moveVector)
    {
        if (IsInTheGrid(tetrimino, moveVector) && !IsCollision(tetrimino, moveVector))
        {
            //tetrimino.PositionInGrid += moveVector;
            return true;
        }

        return false;
    }

    private bool IsInTheGrid(Tetrimino tetrimino, Vector2Int moveVector)
    {
        Vector2Int newPosition = tetrimino.PositionInGrid + moveVector;

        int tetriminoRight = newPosition.x + tetrimino.BoundaryDict[Boundary.Right];
        int tetriminoLeft = newPosition.x;
        int tetriminoTop = Mathf.Abs(newPosition.y) + tetrimino.BoundaryDict[Boundary.Top];
        int tetriminoBottom = Mathf.Abs(newPosition.y);

        if (tetriminoRight <= gridSize.x &&
            tetriminoLeft >= -tetrimino.BoundaryDict[Boundary.Left] &&
            tetriminoBottom <= gridSize.y - tetrimino.BoundaryDict[Boundary.Bottom] &&
            tetriminoTop >= 0)
        {
            return true;
        }

        return false;
    }

    public void AddTetriminoInfo(Tetrimino tetrimino)
    {
        for (int col = 0; col < tetrimino.CurrentArray.GetLength(1); col++)
        {
            for (int row = 0; row < tetrimino.CurrentArray.GetLength(0); row++)
            {
                var x = row + tetrimino.PositionInGrid.x;
                var y = col + Mathf.Abs(tetrimino.PositionInGrid.y);

                if (x >= 0 && x < RowLength && y < ColLength)
                {
                    if (tetrimino.CurrentArray[col, row])
                    {
                        grid[x, y] = tetrimino.CurrentArray[col, row];
                        SetGridColor(x, y, tetrimino.BlockColor);
                    }
                }
            }
        }

        CheckFullLine();

        if (toBeRemoveLineList.Count > 0)
            RemoveLine();
    }

    public bool IsCollision(Tetrimino tetrimino, Vector2Int moveVector)
    {
        for (int col = 0; col < tetrimino.CurrentArray.GetLength(1); col++)
        {
            for (int row = 0; row < tetrimino.CurrentArray.GetLength(0); row++)
            {
                int x = row + tetrimino.PositionInGrid.x + moveVector.x;
                int y = col + Mathf.Abs(tetrimino.PositionInGrid.y + moveVector.y);

                //이미 그리드에 있는지 확인
                if (x < RowLength && x >= 0 && y < ColLength)
                {
                    if (grid[x, y] && tetrimino.CurrentArray[col, row])
                        return true;
                }

                //바닥에 닿을 때
                if (moveVector.y == -1)
                {
                    if (ColLength == Mathf.Abs(tetrimino.PositionInGrid.y) + tetrimino.BoundaryDict[Boundary.Bottom])
                        return true;
                }
            }
        }
        return false;
    }

    private void CheckFullLine()
    {
        int count = 0;
        for (int col = 0; col < ColLength; col++)
        {
            for (int row = 0; row < RowLength; row++)
            {
                if (grid[row, col])
                    count++;
            }

            if (count == RowLength)
                toBeRemoveLineList.Add(col);

            count = 0;
        }
    }

    private void RemoveLine()
    {
        toBeRemoveLineList.Sort();

        int count = toBeRemoveLineList.Count;
        for (int i = 0; i < count; i++)
        {
            int line = toBeRemoveLineList[0];
            for (int col = ColLength - 1; col > 0; col--)
            {
                for (int row = 0; row < RowLength; row++)
                {
                    if (line >= col)
                    {
                        grid[row, col] = grid[row, col - 1];
                        grid[row, col - 1] = false;

                        Color color = GetGridColor(row, col - 1);
                        SetGridColor(row, col, color);
                    }
                }
            }
            toBeRemoveLineList.RemoveAt(0);
        }
    }

    private void SetGridColor(int row, int col, Color color)
    {
        int childCount = (col * RowLength + row);
        container.GetChild(childCount).GetComponent<MeshRenderer>().material.color = color;
    }

    private Color GetGridColor(int row, int col)
    {
        int childCount = (col * RowLength + row);
        return container.GetChild(childCount).GetComponent<MeshRenderer>().material.color;
    }

    public void DebugGrid()
    {
        string s = string.Empty;
        for (int dim2 = 0; dim2 < ColLength; dim2++)
        {
            s += "\n";
            for (int dim1 = 0; dim1 < RowLength; dim1++)
            {
                s += grid[dim1, dim2] ? full : empty;
            }
        }

        Debug.Log(s);
    }
}
