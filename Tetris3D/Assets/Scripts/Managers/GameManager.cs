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

    private bool isStop;

    private UnityEvent<GameStatus> onChangeGameState = new UnityEvent<GameStatus>();

    void Start()
    {
        currentStatus = GameStatus.Title;

        sound = GetComponent<Sound>();
        grid = new Grid();
        
        spawner = new TetriminoSpawner(new Vector3[4] {
            grid.GetSpawnPoint,
            nextTetriminoGroup.GetChild(0).position,
            nextTetriminoGroup.GetChild(1).position,
            nextTetriminoGroup.GetChild(2).position}, 1);
        spawner.ShowBlocks(false);

        isStop = true;

        onChangeGameState.AddListener(UpdateStatus);
        onChangeGameState.Invoke(GameStatus.Title);
    }

    void Update()
    {
        KeyInput();
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
            {
                onChangeGameState.Invoke(GameStatus.Lobby);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F1))
                onChangeGameState.Invoke(GameStatus.Title);

            if (Input.GetKeyDown(KeyCode.F2))
                onChangeGameState.Invoke(GameStatus.Play);

            if (Input.GetKeyDown(KeyCode.F3))
                onChangeGameState.Invoke(GameStatus.Option);
        }
    }

    private void ControllBlock()
    {

    }

    private void UpdateStatus(GameStatus status)
    {
        currentStatus = status;
        SetSceneEnviroment(currentStatus);
        isStop = currentStatus != GameStatus.Play ? true : false;
    }

    private void SetSceneEnviroment(GameStatus status)
    {
        switch(status)
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
}
