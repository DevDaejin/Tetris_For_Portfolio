using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Tetrimino : Block
{
    public TetriminoType TetriminoType { private set; get; }
    public Material Material { get => material; set => material = value; }

    /// <summary>
    /// Grid상의 좌표, *기본적으로 Y가 "음수"로 저장됨으로 주의 필요
    /// </summary>
    public Vector2Int PositionInGrid { set; get; }
    
    /// <summary>
    /// Tetrimino 회전으로 배열 내에서 변경 되는 비어있는 좌,우,상,하 여백을 저장
    /// </summary>
    public Dictionary<Boundary, int> BoundaryDict { private set; get; } = new Dictionary<Boundary, int>(); 

    public bool[,] CurrentArray { get => allArray[rotate]; }
    private bool[][,] allArray;
    private GameObject[,] gameObjectsArray;

    public Color BlockColor { get => blockColor; }
    private ObjectPool<Tetrimino> tetriminoPool;

    private int rotate;
    private int rotateLength;
   
    /// <summary>
    /// Ghost Tetrimino의 Aplha값
    /// </summary>
    private readonly float alpha = .3f;

    public void Create(ObjectPool<Tetrimino> tetriminoPool)
    {
        int maxSizeOfArray = Constant.TetriminoMaxSize;
        gameObjectsArray = new GameObject[maxSizeOfArray, maxSizeOfArray];
        this.tetriminoPool = tetriminoPool;

        for (int row = 0; row < gameObjectsArray.GetLength(0); row++)
        {
            for (int col = 0; col < gameObjectsArray.GetLength(1); col++)
            {
                Vector3 pos = new Vector3(
                        (col * (Constant.CubeScale + Constant.CubeInterval)),
                        (-row * (Constant.CubeScale + Constant.CubeInterval)), 0);

                var o = Utils.CreateCubeBlock(pos, Constant.CubeScale, $"{row + 1} / {col + 1}", transform);
                o.GetComponent<MeshRenderer>().material = Material;
                o.SetActive(false);

                gameObjectsArray[row, col] = o;
            }
        }
    }

    public void Initialize()
    {
        var seed = Enum.GetValues(typeof(TetriminoType));
        TetriminoType = (TetriminoType)seed.GetValue(UnityEngine.Random.Range(0, seed.Length));

        Initialize(TetriminoType);
    }

    public void Initialize(TetriminoType type)
    {
        rotate = 0;
        TetriminoType = type;
        PositionInGrid = Vector2Int.zero;
        
        Set(TetriminoType);
    }

    private void Set(TetriminoType tetriminoType)
    {
        switch (tetriminoType)
        {
            case TetriminoType.I:
                Set(Color.cyan, TetriminoData.IArray);
                break;
            case TetriminoType.O:
                Set(Color.yellow, TetriminoData.OArray);
                break;
            case TetriminoType.T:
                Set(Constant.Purple, TetriminoData.TArray);
                break;
            case TetriminoType.S:
                Set(Color.green, TetriminoData.SArray);
                break;
            case TetriminoType.Z:
                Set(Color.red, TetriminoData.ZArray);
                break;
            case TetriminoType.L:
                Set(Constant.Orange, TetriminoData.LArray);
                break;
            case TetriminoType.J:
                Set(Color.blue, TetriminoData.JArray);
                break;
        }
    }


    private void Set(Color color, bool[][,] allArray)
    {
        blockColor = color;
        blockColor.a = alpha;
        this.allArray = allArray;
        
        SetBlocks();
    }

    private void SetBlocks()
    {
        rotateLength = allArray.GetLength(0);

        OffAllBlock();

        for (int row = 0; row < CurrentArray.GetLength(0); row++)
        {
            for (int col = 0; col < CurrentArray.GetLength(1); col++)
            {
                if (CurrentArray[row, col])
                {
                    Material.color = blockColor;
                    gameObjectsArray[row, col].SetActive(true);
                }
            }
        }

        //Boundary 재설정
        GetMin();
        GetMax();
    }

    public void SetPosition(Vector2Int moveVector)
    {
        PositionInGrid += moveVector;
        transform.position += Utils.MoveScale(Utils.Vector2IntTo3(moveVector));
    }

    public void InitPosition(Vector3 currentPosition, Vector2Int moveVector)
    {
        transform.position = currentPosition;
        PositionInGrid = moveVector;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isCW">CW = Clockwise, 시계방향</param>
    public void Rotate(bool isCW = true)
    {
        if (isCW)
            rotate++;
        else
            rotate--;

        if (rotate >= rotateLength)
            rotate = 0;

        if (rotate < 0)
            rotate = rotateLength - 1;

        SetBlocks();
    }

    public bool[,] NextRotateArray()
    {
        int next = rotate + 1;

        if (next >= rotateLength)
            next = 0;

        return allArray[next]; 
    }

    private void OffAllBlock()
    {
        for (int row = 0; row < gameObjectsArray.GetLength(0); row++)
        {
            for (int col = 0; col < gameObjectsArray.GetLength(1); col++)
            {
                gameObjectsArray[row, col].SetActive(false);
            }
        }
    }
    /// <summary>
    /// Tetrimino마다 배열 Size가 달라 중점이 달라지는데 그것을 고려하여 중점에 대한 조정값 반환.
    /// </summary>
    /// <returns></returns>
    public Vector3 OffsetToCenter()
    {
        float scaleValue = Constant.CubeScale + Constant.CubeInterval;
        float length = CurrentArray.GetLength(0);
        float centerOfArray = length * 0.5f;

        float x = (centerOfArray - scaleValue) * scaleValue;
        float y = (-(centerOfArray - scaleValue) * scaleValue) + ((CurrentArray.GetLength(1) - BoundaryDict[Boundary.Bottom] - BoundaryDict[Boundary.Top]) * scaleValue * 0.5f);

        return new Vector3(x, y, 0);
    }


    public void GetMin()
    {
        BoundaryDict[Boundary.Left] = int.MinValue;
        BoundaryDict[Boundary.Top] = int.MinValue;

        for (int row = 0; row < CurrentArray.GetLength(0); row++)
        {
            for (int col = 0; col < CurrentArray.GetLength(1); col++)
            {
                if (CurrentArray[row, col])
                {
                    if (BoundaryDict[Boundary.Left] == int.MinValue ||
                        BoundaryDict[Boundary.Left] > col)
                        BoundaryDict[Boundary.Left] = col;

                    if (BoundaryDict[Boundary.Top] == int.MinValue ||
                        BoundaryDict[Boundary.Top] > row)
                        BoundaryDict[Boundary.Top] = row;
                }
            }
        }
    }

    public void GetMax()
    {
        BoundaryDict[Boundary.Right] = int.MaxValue;
        BoundaryDict[Boundary.Bottom] = int.MaxValue;

        for (int row = CurrentArray.GetLength(0) - 1; row >= 0; row--)
        {
            for (int col = CurrentArray.GetLength(1) - 1; col >= 0; col--)
            {
                if (CurrentArray[row, col])
                {
                    if (BoundaryDict[Boundary.Right] == int.MaxValue ||
                        BoundaryDict[Boundary.Right] < col)
                        BoundaryDict[Boundary.Right] = col;

                    if (BoundaryDict[Boundary.Bottom] == int.MaxValue ||
                        BoundaryDict[Boundary.Bottom] < row)
                        BoundaryDict[Boundary.Bottom] = row;
                }
            }
        }

        BoundaryDict[Boundary.Right] += 1;
        BoundaryDict[Boundary.Bottom] += 1;
    }

    public void Release()
    {
        if (tetriminoPool != null)
        {
            tetriminoPool.Release(this);
        }
    }
}