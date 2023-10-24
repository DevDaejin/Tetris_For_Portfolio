using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino : Block
{
    [SerializeField] private TetriminoType teriminoType;
    private bool isInitialized = false;
    private GameObject[,] GameObjectsArray = new GameObject[,] {{null, null, null, null},
                                                                {null, null, null, null},
                                                                {null, null, null, null},
                                                                {null, null, null, null}};

    public void InitTetrimino(TetriminoType type)
    {
        teriminoType = type;
        material = new Material(Shader.Find(Constant.LitShaderPath));

        switch (teriminoType)
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
                SetTetrimino(Constant.Purple, new int[4, 4]
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
                SetTetrimino(Constant.Orange, new int[4, 4]
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

        float centerX = array.GetLength(0) * (Constant.CubeScale + Constant.CubeInterval) * -0.5f;
        float centerY = array.GetLength(1) * (Constant.CubeScale + Constant.CubeInterval) * -0.5f;

        for (int row = 0; row < array.GetLength(0); row++)
        {
            for (int col = 0; col < array.GetLength(1); col++)
            {
                if (!isInitialized)
                {
                    var o = Utils.CreateCubeBlock(centerX + (row * (Constant.CubeScale + Constant.CubeInterval)),
                                                  -(centerY + (col * (Constant.CubeScale + Constant.CubeInterval))),
                                                  Constant.CubeScale, parent: transform);

                    o.name = $"{col + 1} / {row + 1}";
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

        if (!isInitialized)
            isInitialized = true;
    }
}
