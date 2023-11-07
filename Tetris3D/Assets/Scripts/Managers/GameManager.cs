using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject titleGroup;
    [SerializeField] private GameObject gameGroup;
    [SerializeField] private GameObject optionGroup;
    [SerializeField] private GameObject resultGroup;
    [SerializeField] private Transform nextTetriminoGroup;
    [SerializeField] private TMP_Text infoText;

    private GameStatus currentStatus;
    private Grid grid;
    private Sound sound;
    private Result result;
    private Option option;

    private TetriminoManager tetriminoManager;
    private Tetrimino currentTetrimino;

    private int level;
    private int score;
    private bool isStop;

    private float accelation;
    private float time = 0;

    private UnityEvent<GameStatus> onChangeGameState = new UnityEvent<GameStatus>();

    private const int tetriminoArraySize = 4;
    private const float normalAccelation = 1;
    private const float fastAccelation = 30;

    private readonly float updateInterval = 1f;
    private readonly float orthoSize = 5.8f;
    private readonly string levelTitle = "Level : ";
    private readonly string scoreTitle = "Score : ";

    void Start()
    {
        Application.targetFrameRate = 60;
        currentStatus = GameStatus.Title;

        result = gameObject.GetComponent<Result>();
        result.OnRegameButtonEvent.AddListener(() => onChangeGameState.Invoke(GameStatus.Play));

        grid = new Grid(Constant.GridSize, Constant.CubeScale, Constant.CubeInterval);
        grid.OnRemoveLine.AddListener(UpdateScore);
        grid.OnGameOver.AddListener(() =>
        {
            result.SetScore(score);
            result.UpdateRank();
            onChangeGameState.Invoke(GameStatus.Result);
        });

        tetriminoManager = gameObject.AddComponent<TetriminoManager>();
        tetriminoManager.Init(new Vector3[tetriminoArraySize]
        { 
            grid.GetSpawnPoint, 
            nextTetriminoGroup.GetChild(0).position, 
            nextTetriminoGroup.GetChild(1).position, 
            nextTetriminoGroup.GetChild(2).position
        }, 
        new Vector2Int(Constant.SpawnRow, 0), tetriminoArraySize);
        
        tetriminoManager.OnUpdateCurretTetrimino.AddListener((t) => currentTetrimino = t);

        sound = GetComponent<Sound>();

        option = GetComponent<Option>();
        option.OnChangedBGM.AddListener(sound.SetBGMVolume);
        option.OnChangedSFX.AddListener(sound.SetSFXVolume);
        option.OnChangedGhostMode.AddListener(tetriminoManager.ShowGhost);
        option.OnCancel.AddListener(EndOption);
        option.OnConfirm.AddListener(EndOption);
        option.Init();

        onChangeGameState.AddListener(ChangeStatus);
        onChangeGameState.Invoke(GameStatus.Title);        

        isStop = true;
        level = 1;
        score = 0;
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

    private void ChangeStatus(GameStatus status)
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
                    UpdateScore(0);

                    grid.InitGrid();
                    grid.ActiveGrid();

                    tetriminoManager.DeleteAllTetrimino();
                    tetriminoManager.CreateAndStart();
                    tetriminoManager.ShowBlocks(true);

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
                    grid.ActiveGrid(false);

                    tetriminoManager.DeleteAllTetrimino();
                    tetriminoManager.ShowBlocks(false);

                    sound.PlayBGM(BGM.Title);
                    ActiveSceneObjectByStatus(activeTitle: true);
                }
                break;
        }
    }

    private void UpdateScore(int removedLine)
    {
        switch(removedLine)
        {
            case 1:
                score += 100;
                sound.PlaySFX(SFX.Line1);
                break;
            case 2:
                score += 300;
                sound.PlaySFX(SFX.Line2);
                break;
            case 3:
                score += 550;
                sound.PlaySFX(SFX.Line3);
                break;
            case 4:
                score += 800;
                sound.PlaySFX(SFX.Line4);
                break;
            default:
                score = 0;
                break;
        }

        level = Mathf.FloorToInt(score * 0.001f) + 1;

        infoText.text = $"{levelTitle}{level}\n{scoreTitle}{score.ToString("N0")}";
    }

    private void ControllTetrimino()
    {
        if (!isStop && currentTetrimino != null)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                accelation = fastAccelation;

            if (Input.GetKeyUp(KeyCode.DownArrow))
                accelation = normalAccelation;

            if (time < (updateInterval - (level * 0.1f)))
                time += Time.deltaTime * accelation;
            else
            {
                MoveTetrimino(Vector2Int.down);
                time = 0;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                MoveTetrimino(Vector2Int.left);

            if (Input.GetKeyDown(KeyCode.RightArrow))
                MoveTetrimino(Vector2Int.right);

            if (Input.GetKeyDown(KeyCode.UpArrow))
                RotateTetrimino();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                time = updateInterval;
                HardDrop();
                sound.PlaySFX(SFX.HardDrop);
            }

            UpdateGhostPosition();
        }
    }

    private void MoveTetrimino(Vector2Int dir)
    {
        if (grid.IsMovingValidation(currentTetrimino, dir))
        {
            currentTetrimino.SetPosition(dir);

            if(dir != Vector2Int.down)
                sound.PlaySFX(SFX.Move);
        }
        else
        {
            if (grid.IsCollision(currentTetrimino, Vector2Int.down))
            {
                grid.AddTetriminoInfo(currentTetrimino);
                sound.PlaySFX(SFX.SoftDrop);
                currentTetrimino.Release();
            }
        }
    }

    private void RotateTetrimino()
    {
        if (grid.IsRotateValidation(currentTetrimino.NextRotateArray(), currentTetrimino.PositionInGrid))
        {
            currentTetrimino.Rotate();

            var kicked = grid.WallKick(currentTetrimino);
            currentTetrimino.SetPosition(kicked);

            UpdateGhostPosition(true);

            if (!grid.IsRotateValidation(currentTetrimino.CurrentArray, currentTetrimino.PositionInGrid))
            {
                currentTetrimino.Rotate(false);
                UpdateGhostPosition(true, false);

                currentTetrimino.SetPosition(-kicked);
            }

            sound.PlaySFX(SFX.Rotate);
        }
    }

    private void UpdateGhostPosition(bool isRotate = false, bool isCW = true)
    {
        if (isRotate)
            tetriminoManager.Ghost.Rotate(isCW);

        var height = grid.CheckHighestBlock(currentTetrimino);
        tetriminoManager.UpdateGhostPosition(height);
    }

    private void HardDrop()
    {
        var height = grid.CheckHighestBlock(currentTetrimino);
        tetriminoManager.HardDrop(height);
    }

    private void EndOption()
    {
        optionGroup.SetActive(false);
        currentStatus = GameStatus.Play;
        isStop = false;
    }

    private void ActiveSceneObjectByStatus(bool activeTitle = false, bool activeGame = false, bool activeResult = false, bool activeOption = false)
    {
        titleGroup.SetActive(activeTitle);
        gameGroup.SetActive(activeGame);
        resultGroup.SetActive(activeResult);
        optionGroup.SetActive(activeOption);
    }
}
