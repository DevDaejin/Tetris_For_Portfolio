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
    private TetriminoSpawner spawner;
    private Sound sound;
    private int level;
    private float updateInterval = 1f;
    private float accelation = 1;
    private float time = 0;
    private float orthoSize = 5.8f;
    private bool isStop;
    private const int tetriminoArraySize = 4;

    private UnityEvent<GameStatus> onChangeGameState = new UnityEvent<GameStatus>();
    void Start()
    {
        currentStatus = GameStatus.Title;

        sound = GetComponent<Sound>();
        grid = new Grid(Constant.GridSize.x, Constant.GridSize.y, Constant.CubeScale, Constant.CubeInterval);

        spawner = new TetriminoSpawner(new Vector3[tetriminoArraySize] {
            grid.GetSpawnPoint,
            nextTetriminoGroup.GetChild(0).position,
            nextTetriminoGroup.GetChild(1).position,
            nextTetriminoGroup.GetChild(2).position}, tetriminoArraySize);

        isStop = true;

        onChangeGameState.AddListener(UpdateStatus);
        onChangeGameState.Invoke(GameStatus.Title);

        Init();
    }

    private void Init()
    {
        level = 0;
        time = 0;
    }

    private void Update()
    {
        KeyInput();        
    }

    private bool MoveTetrimino(Vector2Int dir)
    {
        var currentTetrimino = spawner.GetCurrentTetrimino;
        if (grid.IsMovingValidation(currentTetrimino, dir))
        {
            currentTetrimino.transform.position += MoveScale(new Vector3(dir.x, dir.y, 0));
        }
        else
        {
            if (dir == Vector2Int.down)
            {
                spawner.GetCurrentTetrimino.Release();
                return false;
            }
        }
        return true;
    }

    private void KeyInput()
    {
        ChangeGameStatusByInput();

        if (currentStatus == GameStatus.Play && !isStop)
            ControllBlock();
    }

    private void ChangeGameStatusByInput()
    {
        if (currentStatus == GameStatus.Title)
        {
            if (Input.anyKeyDown)
                onChangeGameState.Invoke(GameStatus.Lobby);
        }
        else
        {
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
    }

    private void ControllBlock()
    {
        if (!isStop && spawner.GetCurrentTetrimino != null)
        {
            if (time < (updateInterval - (level * 0.1f)))
                time += Time.deltaTime * accelation;
            else
            {
                MoveTetrimino(Vector2Int.down);
                time = 0;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTetrimino(Vector2Int.left);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTetrimino(Vector2Int.right);
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                spawner.GetCurrentTetrimino.RotateBlock();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                accelation = 10f;
            }
            if (Input.GetKeyUp(KeyCode.DownArrow))
                accelation = 1;
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
                    spawner.ReleaseAll();
                    spawner.ShowBlocks(false);
                    sound.StopBGM();
                    ActiveSceneObjectByStatus(activeGame: true);
                }
                break;
            case GameStatus.Play:
                {
                    spawner.ReleaseAll();
                    spawner.Start();
                    spawner.ShowBlocks(true);
                    grid.ActiveGrid();
                    sound.PlayBGM(BGM.Game);
                    ActiveSceneObjectByStatus(activeGame: true);
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
                    spawner.ReleaseAll();
                    spawner.ShowBlocks(false);
                    grid.ActiveGrid(false);
                    sound.PlayBGM(BGM.Title);
                    ActiveSceneObjectByStatus(activeTitle: true);
                }
                break;
        }
    }

    private void ActiveSceneObjectByStatus(bool activeTitle = false, bool activeGame = false, bool activeResult = false, bool activeOption = false)
    {
        titleGroup.SetActive(activeTitle);
        gameGroup.SetActive(activeGame);
        resultGroup.SetActive(activeResult);
        optionGroup.SetActive(activeOption);
    }

    private Vector3 MoveScale(Vector3 direction)
    {
        return direction * (Constant.CubeInterval + Constant.CubeScale);
    }
}
