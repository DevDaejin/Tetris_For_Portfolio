using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : Block
{
    private float charInterval = 2;
    private readonly string[] tetris = { "T", "E", "T", "R", "I", "S" };
    private new int[][,] array;

    private void Start()
    {
        SetData();
        GameObject character;
        material = new Material(Shader.Find(shaderPath));

        Vector3 startPosition = Vector3.left * ((tetris.Length - charInterval) * 0.5f);
        for (int i = 0; i < tetris.Length; i++)
        {
            character = new GameObject(tetris[i]);
            character.transform.position = startPosition + (Vector3.right * i * charInterval);
            character.transform.SetParent(transform);

            float centerX = array[i].GetLength(0) * (Constant.cubeScale + Constant.cubeInterval) * -0.5f;
            float centerY = array[i].GetLength(1) * (Constant.cubeScale + Constant.cubeInterval) * -0.5f;

            for (int row = 0; row < array[i].GetLength(0); row++)
            {
                for (int col = 0; col < array[i].GetLength(1); col++)
                {
                    if (array[i][row,col] == 1)
                    {
                        var o = Utils.CreateCubeBlock(centerX + (col * (Constant.cubeScale + Constant.cubeInterval)),
                                                      -(centerY + (row * (Constant.cubeScale + Constant.cubeInterval))),
                                                      Constant.cubeScale, parent: character.transform);

                        o.GetComponent<MeshRenderer>().material = material;
                    }
                }
            }
        }
    }

    private void SetData()
    {
        array = new int[6][,];
        //T
        array[0] = new int[,]
        {
            {1,1,1},
            {0,1,0},
            {0,1,0},
            {0,1,0},
            {0,1,0}
        };
        //E
        array[1] = new int[,]
        {
            {1,1,1},
            {1,0,0},
            {1,1,1},
            {1,0,0},
            {1,1,1},
        };
        //T
        array[2] = new int[,]
         {
            {1,1,1},
            {0,1,0},
            {0,1,0},
            {0,1,0},
            {0,1,0}
         };
        //R
        array[3] = new int[,]
         {
            {1,1,1},
            {1,0,1},
            {1,1,0},
            {1,0,1},
            {1,0,1}
         };
        //I
        array[4] = new int[,]
         {
            {1,1,1},
            {0,1,0},
            {0,1,0},
            {0,1,0},
            {1,1,1}
         };
        //S
        array[5] = new int[,]
         {
            {1,1,1},
            {1,0,0},
            {1,1,1},
            {0,0,1},
            {1,1,1}
         };
    }
}
