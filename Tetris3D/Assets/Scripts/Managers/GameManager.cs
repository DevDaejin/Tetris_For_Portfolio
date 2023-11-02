using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject titleGroup;
    [SerializeField] private GameObject gameGroup;
    [SerializeField] private GameObject optionGroup;
    [SerializeField] private GameObject resultGroup;

    [SerializeField] private Transform nextTetriminoGroup;

    private GameStatus currentStatus;
    private Grid grid;
    private TetriminoManager tetriminoManager;
    private Sound sound;
    private Tetrimino currentTetrimino;


    private int level;
    private bool isStop;

    private float accelation;
    private float time = 0;
    
    private readonly float updateInterval = 1f;
    private readonly float orthoSize = 5.8f;
    private const int tetriminoArraySize = 4;
    private const float normalAccelation = 1;
    private const float fastAccelation = 30;

    private UnityEvent<GameStatus> onChangeGameState = new UnityEvent<GameStatus>();
    void Start()
    {
        Application.targetFrameRate = 60;

        currentStatus = GameStatus.Title;

        sound = GetComponent<Sound>();

        grid = new Grid(Constant.GridSize, Constant.CubeScale, Constant.CubeInterval);

        tetriminoManager = gameObject.AddComponent<TetriminoManager>();
        tetriminoManager.Init(new Vector3[tetriminoArraySize] 
            {
                grid.GetSpawnPoint,
                nextTetriminoGroup.GetChild(0).position,
                nextTetriminoGroup.GetChild(1).position,
                nextTetriminoGroup.GetChild(2).position
            }, 
            new Vector2Int(Constant.SpawnRow, 0), 
            tetriminoArraySize);
        
        tetriminoManager.OnUpdateCurretTetrimino.AddListener((t) => currentTetrimino = t);

        isStop = true;

        onChangeGameState.AddListener(UpdateStatus);
        onChangeGameState.Invoke(GameStatus.Title);

        level = 1;
        time = 0;
        accelation = normalAccelation;
    }

    private void Update()
    {
        ChangeGameStatusByInput();

        if (currentStatus == GameStatus.Play && !isStop)
            ControllTetrimino();
    }

    private void ChangeGameStatusByInput()
    {
        if (currentStatus == GameStatus.Title)
        {
            if (Input.anyKeyDown)
                onChangeGameState.Invoke(GameStatus.Lobby);
        }

        if (Input.GetKeyDown(KeyCode.F1))
            onChangeGameState.Invoke(GameStatus.Title);

        if (Input.GetKeyDown(KeyCode.F2))
            onChangeGameState.Invoke(GameStatus.Play);

        if (Input.GetKeyDown(KeyCode.F3))
            onChangeGameState.Invoke(GameStatus.Option);

        if (Input.GetKeyDown(KeyCode.F4))
        {
            Camera.main.orthographic = Camera.main.orthographic ? false : true;
            Camera.main.orthographicSize = orthoSize;
        }
    }

    private void UpdateStatus(GameStatus status)
    {
        currentStatus = status;
        SetSceneEnviroment(currentStatus);
        isStop = currentStatus != GameStatus.Play;
    }

    private void SetSceneEnviroment(GameStatus status)
    {
        switch (status)
        {
            case GameStatus.Lobby:
                {
                    tetriminoManager.DeleteAllTetrimino();
                    tetriminoManager.ShowBlocks(false);

                    sound.StopBGM();

                    ActiveSceneObjectByStatus(activeGame: true);
                }
                break;
            case GameStatus.Play:
                {
                    level = 1;
                    
                    grid.InitGrid();
                    grid.ActiveGrid();
                    
                    tetriminoManager.DeleteAllTetrimino();
                    tetriminoManager.CreateAndStart();
                    tetriminoManager.ShowBlocks(true);

                    sound.PlayBGM(BGM.Game);

                    ActiveSceneObjectByStatus(activeGame: true);

                    UpdateGhostStatus();
                }
                break;
            case GameStatus.Result:
                {
                    sound.PlayBGM(BGM.Result);
                    ActiveSceneObjectByStatus(activeGame: true, activeResult: true);
                }
                break;
            case GameStatus.Option:
                {
                    ActiveSceneObjectByStatus(activeGame: true, activeOption: true);
                }
                break;
            case GameStatus.Title:
            default:
                {
                    grid.ActiveGrid(false);

                    tetriminoManager.DeleteAllTetrimino();
                    tetriminoManager.ShowBlocks(false);

                    sound.PlayBGM(BGM.Title);
                    ActiveSceneObjectByStatus(activeTitle: true);
                }
                break;
        }
    }

    private void ControllTetrimino()
    {
        if (!isStop && currentTetrimino != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                MoveTetrimino(Vector2Int.left);

            if (Input.GetKeyDown(KeyCode.RightArrow))
                MoveTetrimino(Vector2Int.right);

            if (Input.GetKeyUp(KeyCode.UpArrow))
                RotateTetrimino();

            if (Input.GetKeyDown(KeyCode.DownArrow))
                accelation = fastAccelation;

            if (Input.GetKeyUp(KeyCode.DownArrow))
                accelation = normalAccelation;

            if (time < (updateInterval - (level * 0.1f)))
            {
                time += Time.deltaTime * accelation;
            }
            else
            {
                MoveTetrimino(Vector2Int.down);
                time = 0;
            }
        }
    }

    private void MoveTetrimino(Vector2Int dir)
    {
        if (grid.IsMovingValidation(currentTetrimino, dir))
        {
            currentTetrimino.SetPosition(dir);
            UpdateGhostStatus();
        }
        else
        {
            if (grid.IsCollision(currentTetrimino, Vector2Int.down))
            {
                grid.AddTetriminoInfo(currentTetrimino);
                currentTetrimino.Release();
            }
        }
    }

    private void RotateTetrimino()
    {
        if (grid.IsRotateValidation(currentTetrimino.NextRotateArray(), currentTetrimino.PositionInGrid))
        {
            var kicked = grid.WallKick(currentTetrimino);
            currentTetrimino.SetPosition(kicked);

            currentTetrimino.Rotate();
            UpdateGhostStatus(true);

            if (!grid.IsRotateValidation(currentTetrimino.CurrentArray, currentTetrimino.PositionInGrid))
            {
                currentTetrimino.Rotate(false);
                UpdateGhostStatus(true, false);

                currentTetrimino.SetPosition(-kicked);
            }
        }
    }

    private void UpdateGhostStatus(bool isRotate = false, bool isCW = true)
    {
        if (isRotate)
            tetriminoManager.Ghost.Rotate(isCW);

        var height = grid.CheckHighestBlock(currentTetrimino);
        tetriminoManager.UpdateGhostPosition(height);
    }

    private void ActiveSceneObjectByStatus(bool activeTitle = false, bool activeGame = false, bool activeResult = false, bool activeOption = false)
    {
        titleGroup.SetActive(activeTitle);
        gameGroup.SetActive(activeGame);
        resultGroup.SetActive(activeResult);
        optionGroup.SetActive(activeOption);
    }
}
