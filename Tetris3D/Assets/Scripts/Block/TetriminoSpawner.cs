using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TetriminoSpawner
{
    private GameObject poolContainer;
    private IObjectPool<Tetrimino> tetriminoPool;
    private GameObject currentTetrimino;
    private Queue<GameObject> queueTetrimino = new Queue<GameObject>();
    private Vector3[] tetriminoPositions = new Vector3[4];
    private Vector3 blockIOffset;//block I
    private Vector3 blockOOffset;//block O
    private Vector3 blockOtherOffset;//other block

    public Tetrimino GetCurrentTetrimino { get => currentTetrimino.GetComponent<Tetrimino>(); }

    private readonly string poolName = "TetriminoPool";
    private readonly string tetriminoName = "Tetrimino";

    private int test = 0;
    public TetriminoSpawner(Vector3[] spawnPoint,int initPoolSize)
    {
        Array.Copy(spawnPoint, tetriminoPositions, spawnPoint.Length);

        poolContainer = new GameObject(poolName);
        poolContainer.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        tetriminoPool = new ObjectPool<Tetrimino>(Create, Get, Release, Destroy, defaultCapacity : initPoolSize);

        blockIOffset = new Vector3(Constant.CubeScale * 0.5f, -Constant.CubeScale, 0);
        blockOOffset = new Vector3(Constant.CubeScale * 0.5f, -Constant.CubeScale * 0.5f, 0);
        blockOtherOffset = new Vector3(Constant.CubeScale, -Constant.CubeScale * 0.5f, 0);

        ShowBlocks(false);
    }

    public void Start()
    {
        ReleaseAll();
        Vector3 offset = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            tetriminoPool.Get();
            var child = poolContainer.transform.GetChild(i);

            if (i != 0)
                offset = Offset(child.GetComponent<Tetrimino>());

            child.position = tetriminoPositions[i] + offset;
        }

        currentTetrimino = queueTetrimino.Dequeue();
    }

    public void ShowBlocks(bool isShow) => poolContainer.SetActive(isShow);

    private Tetrimino Create()
    {
        Tetrimino t = new GameObject(tetriminoName + (test++).ToString()).AddComponent<Tetrimino>();
        t.transform.SetParent(poolContainer.transform);
        t.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        return t;
    }

    private void Get(Tetrimino tetrimino)
    {
        tetrimino.gameObject.SetActive(true);
        tetrimino.SetTetrimino();
        queueTetrimino.Enqueue(tetrimino.gameObject);
    }

    private void Release(Tetrimino tetrimino)
    {
        tetrimino.gameObject.SetActive(false);
    }

    private void Destroy(Tetrimino tetrimino)
    {
        Destroy(tetrimino);
    }

    public IObjectPool<Tetrimino> GetPool
    {
        get
        {
            if (tetriminoPool != null)
                return tetriminoPool;

            return null;
        }
    }

    public void Update(bool isReleaseCall = false)
    {
        currentTetrimino = queueTetrimino.Dequeue();
        currentTetrimino.transform.position = tetriminoPositions[0];

        tetriminoPool.Get();

        Tetrimino t;
        for (int i = 1; i < queueTetrimino.Count; i++)
        {
            t = queueTetrimino.ToArray()[i].GetComponent<Tetrimino>();
            t.transform.position = tetriminoPositions[i] + Offset(t.GetComponent<Tetrimino>());
        }
    }

    public void ReleaseAll()
    {
        int count = queueTetrimino.Count;
        for (int i = 0; i < count; i++)
        {
            var o = queueTetrimino.Dequeue();
            tetriminoPool.Release(o.GetComponent<Tetrimino>());
        }
    }

    private Vector3 Offset(Tetrimino tetrimino)
    {
        if (tetrimino.GetTetriminoType == TetriminoType.I)
            return blockIOffset;//block I
        else if (tetrimino.GetTetriminoType == TetriminoType.O)
            return blockOOffset;//block O
        else
            return blockOtherOffset;//other block
    }
}
