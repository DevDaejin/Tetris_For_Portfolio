using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TetriminoSpawner
{
    private GameObject poolContainer;
    private ObjectPool<Tetrimino> tetriminoPool;
    private Queue<Tetrimino> queueTetrimino = new Queue<Tetrimino>();
    private Tetrimino currentTetrimino;
    private int poolSize;
    private Vector3[] tetriminoPositions;

    public Tetrimino GetCurrentTetrimino 
    {
        get
        {
            if (currentTetrimino == null)
                return null;

            return currentTetrimino;
        }
    }

    private readonly string poolName = "TetriminoPool";
    private readonly string tetriminoName = "Tetrimino";

    private int test = 0;
    public TetriminoSpawner(Vector3[] spawnPoints, int initPoolSize)
    {
        poolSize = initPoolSize;
        tetriminoPositions = spawnPoints;
        poolContainer = new GameObject(poolName);
        poolContainer.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        tetriminoPool = new ObjectPool<Tetrimino>(Create, Get, Release, Destroy, defaultCapacity : initPoolSize);

        ShowBlocks(false);
    }

    public void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            tetriminoPool.Get();
        }
        Update(false);
    }

    public void ShowBlocks(bool isShow) => poolContainer.SetActive(isShow);

    private Tetrimino Create()
    {
        var tetrimino = new GameObject(tetriminoName + (test++).ToString()).AddComponent<Tetrimino>();
        tetrimino.transform.SetParent(poolContainer.transform);
        tetrimino.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        tetrimino.Initialize(tetriminoPool);

        return tetrimino;
    }

    private void Get(Tetrimino tetrimino)
    {
        tetrimino.SetTetrimino();
        queueTetrimino.Enqueue(tetrimino);
    }

    private void Release(Tetrimino tetrimino)
    {
        currentTetrimino = null;
        tetrimino.gameObject.SetActive(false);
        Update();
    }

    private void Destroy(Tetrimino tetrimino)
    {
        Destroy(tetrimino);
    }

    public void Update(bool isGet = true)
    {
        if (queueTetrimino.Count == 0)
            return;

        if (isGet)
            tetriminoPool.Get();

        Tetrimino t;
        for (int i = 0; i < queueTetrimino.Count; i++)
        {
            t = queueTetrimino.ToArray()[i].GetComponent<Tetrimino>();
            var offset = new Vector3(t.GetWidth(), t.GetHeight()) * (Constant.CubeScale + Constant.CubeInterval);
            t.transform.position = tetriminoPositions[i] - offset;
            t.gameObject.SetActive(true);
        }

        if (currentTetrimino == null)
        {
            currentTetrimino = queueTetrimino.Dequeue();
            currentTetrimino.transform.position = tetriminoPositions[0];
            currentTetrimino.gameObject.SetActive(true);
        }
    }

    public void ReleaseAll()
    {
        int count = queueTetrimino.Count;
        for (int i = 0; i < count; i++)
        {
            var o = queueTetrimino.Dequeue();
            o.GetComponent<Tetrimino>().Release();
        }
        queueTetrimino.Clear();
    }
}
