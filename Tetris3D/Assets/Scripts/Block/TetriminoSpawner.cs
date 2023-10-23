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

    public TetriminoSpawner(Vector3[] spawnPoint,int initPoolSize)
    {
        Array.Copy(spawnPoint, tetriminoPositions, spawnPoint.Length);

        poolContainer = new GameObject(poolName);
        poolContainer.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        tetriminoPool = new ObjectPool<Tetrimino>(Create, Get, Release, Destroy, defaultCapacity : initPoolSize);

        for (int i = 0; i < 4; i++)
        {
            tetriminoPool.Get();
            var child = poolContainer.transform.GetChild(i);
            child.position = tetriminoPositions[i];
            queueTetrimino.Enqueue(child.gameObject);
        }

        currentTetrimino = queueTetrimino.Dequeue();
    }

    private Tetrimino Create()
    {
        Tetrimino t = new GameObject(tetriminoName).AddComponent<Tetrimino>();
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
        Update();
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
        
        for (int i = 1; i < tetriminoPositions.Length; i++)
        {
            Debug.Log($"{queueTetrimino.ToArray().Length} {tetriminoPositions.Length}");
            queueTetrimino.ToArray()[i].transform.position = tetriminoPositions[i];
        }
    }
}
