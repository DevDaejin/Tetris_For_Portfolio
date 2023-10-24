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

    private readonly string poolName = "TetriminoPool";
    private readonly string tetriminoName = "Tetrimino";

    private int test = 0;
    public TetriminoSpawner(Vector3[] spawnPoint,int initPoolSize)
    {
        Array.Copy(spawnPoint, tetriminoPositions, spawnPoint.Length);

        poolContainer = new GameObject(poolName);
        poolContainer.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        tetriminoPool = new ObjectPool<Tetrimino>(Create, Get, Release, Destroy, defaultCapacity : initPoolSize);
    }

    public void Start()
    {
        ReleaseAll();

        for (int i = 0; i < 4; i++)
        {
            tetriminoPool.Get();
            var child = poolContainer.transform.GetChild(i);
            child.position = tetriminoPositions[i];
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

        var seed = Enum.GetValues(typeof(TetriminoType));
        TetriminoType type = (TetriminoType)seed.GetValue(UnityEngine.Random.Range(0, seed.Length));

        tetrimino.InitTetrimino(type);
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

        GameObject o;
        for (int i = 1; i < queueTetrimino.Count; i++)
        {
            o = queueTetrimino.ToArray()[i];
            o.transform.position = tetriminoPositions[i];
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
}
