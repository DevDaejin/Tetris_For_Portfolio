using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private Transform container;
    private readonly string rootName = "Grid";

    private Vector3 spawnPoint;
    public Vector3 GetSpawnPoint { get => spawnPoint; }
    public Grid()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        container = new GameObject(rootName).transform;
        container.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        float positionX;
        float positionY;

        float xHalf = Constant.GridSize.x * 0.5f;
        float yHalf = Constant.GridSize.y * 0.5f;

        for (float x = 0; x < Constant.GridSize.x; x++)
        {
            for (float y = 0; y < Constant.GridSize.y; y++)
            {
                positionX = CalcuratePosition(x, Constant.cubeScale, xHalf, Constant.cubeInterval);
                positionY = CalcuratePosition(y, Constant.cubeScale, yHalf, Constant.cubeInterval);

                Utils.CreateCubeBlock(positionX, positionY, Constant.cubeScale, $"{x + 1} {y + 1} tile", container);

                if (x == (Constant.GridSize.x * 0.5f) && y == (Constant.GridSize.y - 2))
                    spawnPoint = new Vector3(positionX, positionY);
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
        //��� �� ������ - ���� ���߱� ���� ��
        //Scale �� �ݿ� x + Cube�� �̰� - (��ü ũ���� �� + �̰� �Ÿ��� ���� �������� �� + Cube Size�� ��)
        return (index * scale) + (index * offset) - ((halfOfMaxSize * scale) + (halfOfMaxSize * offset) - (scale * 0.5f));
    }
}
