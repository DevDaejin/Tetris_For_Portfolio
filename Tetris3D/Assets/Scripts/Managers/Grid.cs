using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public Vector3 GetSpawnPoint { get => spawnPoint; }
    
    private Vector3 spawnPoint;
    private Transform container;
    private readonly string rootName = "Grid";
    private float containerOffset = 0.1f;

    public Grid()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        container = new GameObject(rootName).transform;
        container.SetPositionAndRotation(Vector3.forward * containerOffset, Quaternion.identity);

        float positionX;
        float positionY;

        float xHalf = Constant.GridSize.x * 0.5f;
        float yHalf = Constant.GridSize.y * 0.5f;

        GameObject o;

        for (float x = 0; x < Constant.GridSize.x; x++)
        {
            for (float y = 0; y < Constant.GridSize.y; y++)
            {
                positionX = CalcuratePosition(x, Constant.CubeScale, xHalf, Constant.CubeInterval);
                positionY = CalcuratePosition(y, Constant.CubeScale, yHalf, Constant.CubeInterval);

                o = Utils.CreateCubeBlock(positionX, positionY, Constant.CubeScale, $"{x + 1} {y + 1} tile", container);
                var m = o.GetComponent<MeshRenderer>();
                m.material = new Material(Shader.Find(Constant.LitShaderPath));
                m.material.color = Color.gray;

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
        return (index * scale) + (index * offset) - ((halfOfMaxSize * scale) + (halfOfMaxSize * offset) - ((scale + offset) * 0.5f));
    }
}
