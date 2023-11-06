using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Grid
{
    public Vector3 GetSpawnPoint { get => spawnPoint; }
    private Vector3 spawnPoint;

    private List<int> toBeRemoveLineList = new List<int>();

    private float containerZOffset = 0.1f;

    private bool[,] grid;
    private float gridScale;
    private float gridInterval;

    public int Width { private set; get; }
    public int Height { private set; get; }

    public UnityEvent OnGameOver = new UnityEvent();

    private Shader gridShader;
    private Transform container;

    private readonly int deadLine = 2;
    private readonly string containerName = "Grid";
    private readonly string empty = " ○ ";
    private readonly string full = " ● ";

    public Grid(Vector2Int gridSize, float gridScale, float gridInterval)
    {
        this.gridScale = gridScale;
        this.gridInterval = gridInterval;

        Width = gridSize.x;
        Height = gridSize.y;

        gridShader = Shader.Find(Constant.LitShaderPath);
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        container = new GameObject(containerName).transform;
        container.SetPositionAndRotation(Vector3.forward * containerZOffset, Quaternion.identity);

        grid = new bool[Width, Height];

        float gridWidthHalf = Width * 0.5f;
        float gridHeightHalf = Height * 0.5f;

        for (int col = 0; col < Height; col++)
        {
            for (int row = 0; row < Width; row++)
            {
                grid[row, col] = false;

                float positionX = CalcuratePosition(row, gridWidthHalf);
                float positionY = -CalcuratePosition(col, gridHeightHalf);
                var pos = new Vector3(positionX, positionY, 0);
                var o = Utils.CreateCubeBlock(pos, gridScale, $"{row + 1} {col + 1}", container);

                var m = o.GetComponent<MeshRenderer>();
                m.material = new Material(gridShader);
                m.material.color = Color.gray;

                if (row == Constant.SpawnRow && col == 0)
                    spawnPoint = o.transform.localPosition;
            }
        }

        ActiveGrid(false);
    }

    public void InitGrid()
    {
        grid = new bool[Width, Height];
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
                if (x > 0 && x < Width && y > 0 && y < Height)
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
        bool isMovable = true;
        Vector2Int moveVector = Vector2Int.zero;

        while (isMovable)
        {
            moveVector += Vector2Int.down;
            isMovable = IsMovingValidation(tetrimino, moveVector);
        }

        return moveVector.y + 1;
    }

    public bool IsMovingValidation(Tetrimino tetrimino, Vector2Int moveVector)
    {
        if (IsInTheGrid(tetrimino, moveVector) && !IsCollision(tetrimino, moveVector))
        {
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

        if (tetriminoRight <= Width &&
            tetriminoLeft >= -tetrimino.BoundaryDict[Boundary.Left] &&
            tetriminoBottom <= Height - tetrimino.BoundaryDict[Boundary.Bottom] &&
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

                if (x >= 0 && x < Width && y < Height)
                {
                    if (tetrimino.CurrentArray[col, row])
                    {
                        grid[x, y] = tetrimino.CurrentArray[col, row];
                        SetGridColor(x, y, tetrimino.BlockColor);

                        if (x >= Constant.SpawnRow && x <= Constant.SpawnRow + 4 &&
                           y >= 0 && y <= 1)
                            OnGameOver.Invoke();
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
                if (x < Width && x >= 0 && y < Height)
                {
                    if (grid[x, y] && tetrimino.CurrentArray[col, row])
                    {
                        return true;
                    }
                }

                //바닥에 닿을 때
                if (moveVector.y < 0)
                {
                    if (Height == Mathf.Abs(tetrimino.PositionInGrid.y) + tetrimino.BoundaryDict[Boundary.Bottom])
                        return true;
                }
            }
        }
        return false;
    }

    private void CheckFullLine()
    {
        int count = 0;
        for (int col = 0; col < Height; col++)
        {
            for (int row = 0; row < Width; row++)
            {
                if (grid[row, col])
                    count++;
            }

            if (count == Width)
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
            for (int col = Height - 1; col > 0; col--)
            {
                for (int row = 0; row < Width; row++)
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
        int childCount = (col * Width + row);
        container.GetChild(childCount).GetComponent<MeshRenderer>().material.color = color;
    }

    private Color GetGridColor(int row, int col)
    {
        int childCount = (col * Width + row);
        return container.GetChild(childCount).GetComponent<MeshRenderer>().material.color;
    }

    public void DebugGrid()
    {
        string s = string.Empty;
        for (int col = 0; col < Height; col++)
        {
            s += "\n";
            for (int row = 0; row < Width; row++)
            {
                s += grid[row, col] ? full : empty;
            }
        }

        Debug.Log(s);
    }
}
