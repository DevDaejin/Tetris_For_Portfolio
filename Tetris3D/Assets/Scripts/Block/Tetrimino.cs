using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino : Block
{
    public int[,] GetArray { get => array; }

    [SerializeField] private TetriminoType type;
    private bool isInit = false;
    private int[,] array;
    private GameObject[,] GameObjectsArray;
    private Color color = Color.white;
    private Material material;

    private readonly Color purple = new Color(128 / 255, 0, 128 / 255);
    private readonly Color orange = new Color(1, 127 / 255, 0);
    private readonly string shaderPath = "Universal Render Pipeline/Lit";
    private readonly string colorPropertyName = "_BaseColor";

    public void InitTetrimino(TetriminoType type)
    {
        this.type = type;
        material = new Material(Shader.Find(shaderPath));

        GameObjectsArray = new GameObject[,] {{null, null, null, null},
                                              {null, null, null, null},
                                              {null, null, null, null},
                                              {null, null, null, null}};

        switch (type)
        {
            case TetriminoType.I:
                SetTetrimino(Color.cyan, new int[4, 4]
                    {
                        {0,0,0,0},
                        {1,1,1,1},
                        {0,0,0,0},
                        {0,0,0,0}
                    });
                break;
            case TetriminoType.O:
                SetTetrimino(Color.yellow, new int[4, 4]
                    {
                        {0,0,0,0},
                        {0,1,1,0},
                        {0,1,1,0},
                        {0,0,0,0}
                    });
                break;
            case TetriminoType.T:
                SetTetrimino(purple, new int[4, 4]
                    {
                        {0,0,0,0},
                        {1,1,1,0},
                        {0,1,0,0},
                        {0,0,0,0}
                    });
                break;
            case TetriminoType.S:
                SetTetrimino(Color.green, new int[4, 4]
                    {
                        {0,0,0,0},
                        {0,1,1,0},
                        {1,1,0,0},
                        {0,0,0,0}
                    });
                break;
            case TetriminoType.Z:
                SetTetrimino(Color.red, new int[4, 4]
                    {
                        {0,0,0,0},
                        {1,1,0,0},
                        {0,1,1,0},
                        {0,0,0,0}
                    });
                break;
            case TetriminoType.L:
                SetTetrimino(orange, new int[4, 4]
                    {
                        {0,0,0,0},
                        {1,1,1,0},
                        {1,0,0,0},
                        {0,0,0,0}
                    });
                break;
            case TetriminoType.J:
                SetTetrimino(Color.blue, new int[4, 4]
                    {
                        {0,0,0,0},
                        {1,0,0,0},
                        {1,1,1,0},
                        {0,0,0,0}
                    });
                break;
        }
    }

    private void SetTetrimino(Color color, int[,] array)
    {
        this.color = color;
        this.array = array;

        float centerX = array.GetLength(0) * (Constant.cubeScale + Constant.cubeInterval) * -0.5f;
        float centerY = array.GetLength(1) * (Constant.cubeScale + Constant.cubeInterval) * -0.5f;

        for (int col = array.GetLength(1) - 1; col >= 0; col--)
        {
            for (int row = 0; row < array.GetLength(0); row++)
            {
                if (!isInit)
                {
                    var o = Utils.CreateCubeBlock(centerX + (row * (Constant.cubeScale + Constant.cubeInterval)),
                                                  centerY + (col * (Constant.cubeScale + Constant.cubeInterval)),
                                                  Constant.cubeScale, parent: transform);

                    o.name = $"{row + 1} / {col + 1}";
                    o.gameObject.SetActive(false);
                    GameObjectsArray[col, row] = o;
                }

                if (array[col, row] == 1)
                {
                    material.SetColor(colorPropertyName, color);
                    GameObjectsArray[col, row].GetComponent<MeshRenderer>().material = material;
                    GameObjectsArray[col, row].SetActive(true);
                }
                else
                {
                    GameObjectsArray[col, row].SetActive(false);
                }
            }
        }

        if (!isInit)
            isInit = true;
    }
}
