using UnityEngine;

public class Constant
{
    public static Vector2Int GridSize = new Vector2Int(10, 20);
    
    public static Color Purple = new Color(0.5f, 0, 0.5f);
    public static Color Orange = new Color(1, 0.5f, 0);
    
    public const float CubeScale = 0.4f;
    public const float CubeInterval = 0.03f;

    public const int SpawnRow = 4;
    public const int TetriminoSize = 4;
    public const string LitShaderPath = "Universal Render Pipeline/Lit";
}