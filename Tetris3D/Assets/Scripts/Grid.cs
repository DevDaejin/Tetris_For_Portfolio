using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private Transform container;
    private readonly float offset = 0.03f;
    private readonly float cubeScale = 0.4f;
    private readonly string rootName = "Grid";
    public void GenerateGrid()
    {
        container = new GameObject(rootName).transform;
        container.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        float positionX = 0;
        float positionY = 0;

        float xHalf = Constant.GridSize.x * 0.5f;
        float yHalf = Constant.GridSize.y * 0.5f;

        for (float x = 0; x < Constant.GridSize.x; x++)
        {
            for (float y = 0; y < Constant.GridSize.y; y++)
            {
                positionX = CalcuratePosition(x, cubeScale, xHalf, offset);
                positionY = CalcuratePosition(y, cubeScale, yHalf, offset);

                Utils.CreateCubeBlock(positionX, positionY, cubeScale, $"{x + 1} {y + 1} tile", container);
            }
        }
    }

    private float CalcuratePosition(float index, float scale, float halfOfMaxSize, float offset)
    {
        //계산 된 포지션 - 센터 맞추기 위한 값
        //Scale 값 반영 x + Cube간 이격 - (전체 크기의 반 + 이격 거리로 인해 벌어진값 반 + Cube Size의 반)
        return (index * scale) + (index * offset) - ((halfOfMaxSize * scale) + (halfOfMaxSize * offset) - (scale * 0.5f));
    }
}
