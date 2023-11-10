using UnityEngine;

public class Constant
{
    public static Vector2Int GridSize = new Vector2Int(10, 20);

    public static Color Purple = new Color(0.5f, 0, 0.5f);
    public static Color Orange = new Color(1, 0.5f, 0);

    public const float CubeScale = 0.4f;
    public const float CubeInterval = 0.03f;

    public const int SpawnRow = 4;
    public const int TetriminoMaxSize = 4;
    public const string LitShaderPath = "Universal Render Pipeline/Lit";


    //T
    public static bool[,] TArray = new bool[5, 3]
    {
        {true,  true,   true},
        {false, true,   false},
        {false, true,   false},
        {false, true,   false},
        {false, true,   false}
    };
    //E
    public static bool[,] EArray = new bool[5, 3]
    {
        {true,  true,   true},
        {true,  false,  false},
        {true,  true,   true},
        {true,  false,  false},
        {true,  true,   true},
    };
    //R
    public static bool[,] RArray = new bool[5, 3]
     {
        {true,  true,   true},
        {true,  false,  true},
        {true,  true,   false},
        {true,  false,  true},
        {true,  false,  true}
    };
    //I
    public static bool[,] IArray = new bool[5, 3]
     {
        {true,  true,   true},
        {false, true,   false},
        {false, true,   false},
        {false, true,   false},
        {true,  true,   true}
    };
    //S
    public static bool[,] SArray = new bool[5, 3]
     {
        {true,  true,   true},
        {true,  false,  false},
        {true,  true,   true},
        {false, false,  true},
        {true,  true,   true}
    };
}