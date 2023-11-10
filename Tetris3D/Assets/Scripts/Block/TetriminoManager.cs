using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class TetriminoManager : MonoBehaviour
{
    private GameObject poolContainer;
    private int poolSize;
    private ObjectPool<Tetrimino> tetriminoPool;
    //Unity에서 제공하는 ObjectPool의 Clear가 정상 작동하지 않아 별도 Queue를 만들어 관리
    private Queue<Tetrimino> queueTetrimino = new Queue<Tetrimino>();

    //Tetrimino 생성 지점, 대기열 1~3에 대한 좌표값 배열
    private Vector3[] tetriminoPositions;
    private Vector2Int spawnPositionInGrid;

    //Material을 동적 생성하려 했으나..
    //기본 Lit shader에서는 Surface type을 동적 변환이 불가함
    private Material tetriminoMat;
    private Material ghostMat;
    
    public Tetrimino Ghost { private set; get; }
    private Tetrimino currentTetrimino;
    public UnityEvent<Tetrimino> OnUpdateCurretTetrimino = new UnityEvent<Tetrimino>();

    private readonly string poolName = "TetriminoPool";
    private readonly string tetriminoName = "Tetrimino";
    private readonly string ghostName = "Ghost";
    private readonly string ghostMaterialPath = "Materials/GhostMat";

    public void InitManager(Vector3[] spawnPoints, Vector2Int spawnPositionInGrid, int initPoolSize)
    {
        tetriminoMat = new Material(Shader.Find(Constant.LitShaderPath));
        ghostMat = Resources.Load<Material>(ghostMaterialPath);

        poolSize = initPoolSize;
        poolContainer = new GameObject(poolName);
        poolContainer.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        tetriminoPool = new ObjectPool<Tetrimino>(Create, Get, Release, Remove, defaultCapacity : initPoolSize);
        
        tetriminoPositions = spawnPoints;
        this.spawnPositionInGrid = spawnPositionInGrid;

        OnUpdateCurretTetrimino.AddListener(InitializeGhost);
        ShowBlocks(false);
    }

    public void InitTetriminos()
    {
        Ghost = Create(ghostMat);
        Ghost.gameObject.name = ghostName;

        for (int i = 0; i < poolSize; i++)
        {
            tetriminoPool.Get();
        }

        currentTetrimino = null;
        OnUpdateCurretTetrimino.Invoke(currentTetrimino);
        UpdateTetrisWaittingQueue(false);
    }

    public void ShowBlocks(bool isShow) => poolContainer.SetActive(isShow);
    public void ShowGhost(bool isShow) => Ghost?.gameObject.SetActive(isShow);

    private Tetrimino Create()
    {
        return Create(tetriminoMat);
    }

    private Tetrimino Create(Material mat)
    {
        var tetrimino = new GameObject(tetriminoName.ToString()).AddComponent<Tetrimino>();
        tetrimino.transform.SetParent(poolContainer.transform);
        tetrimino.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        tetrimino.Material = new Material(mat);
        tetrimino.Create(tetriminoPool);

        return tetrimino;
    }

    private void Get(Tetrimino tetrimino)
    {
        tetrimino.Initialize();
        queueTetrimino.Enqueue(tetrimino);
    }

    private void Release(Tetrimino tetrimino)
    {
        currentTetrimino = null;
        OnUpdateCurretTetrimino.Invoke(currentTetrimino);
        tetrimino.gameObject.SetActive(false);
        UpdateTetrisWaittingQueue();
    }

    private void Remove(Tetrimino tetrimino)
    {
        Destroy(tetrimino.gameObject);
    }

    private void UpdateTetrisWaittingQueue(bool isGet = true)
    {
        if (queueTetrimino.Count == 0)
            return;

        if (isGet)
            tetriminoPool.Get();

        var tArray = queueTetrimino.ToArray();
        for (int i = 0; i < queueTetrimino.Count; i++)
        {
            tArray[i].transform.position = tetriminoPositions[i] - tArray[i].OffsetToCenter();
            tArray[i].gameObject.SetActive(true);
        }

        if (currentTetrimino == null)
        {
            currentTetrimino = queueTetrimino.Dequeue();
            currentTetrimino.InitPosition(tetriminoPositions[0], spawnPositionInGrid);
            currentTetrimino.gameObject.SetActive(true);
            
            OnUpdateCurretTetrimino.Invoke(currentTetrimino);
        }
    }

    private void InitializeGhost(Tetrimino tetrimino)
    {
        if (tetrimino == null)
            return;

        Ghost.Initialize(tetrimino.TetriminoType);
    }

    public void UpdateGhostPosition(int height)
    {
        Ghost.transform.position = currentTetrimino.transform.position;
        Ghost.SetPosition(Vector2Int.up * height);
    }

    public void HardDrop(int height)
    {
        currentTetrimino.SetPosition(Vector2Int.up * height);
    }

    public void Hold()
    {

    }

    public void DeleteAllTetrimino()
    {
        if(Ghost != null)
            Destroy(Ghost.gameObject);

        poolContainer.GetComponentsInChildren<Tetrimino>().ToList().ForEach((t) => Destroy(t.gameObject));
        queueTetrimino.Clear();
        tetriminoPool.Clear();
    }
}
