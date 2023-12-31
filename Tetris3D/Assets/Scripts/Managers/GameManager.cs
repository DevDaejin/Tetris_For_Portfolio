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
    [SerializeField] private Transform holdTransform;
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
    private bool isHolding;
    private float accelation;
    private float downTime = 0;
    private float repeatTime = 0;

    private UnityEvent<GameStatus> onChangeGameState = new UnityEvent<GameStatus>();

    private const int tetriminoArraySize = 4;
    private const float normalAccelation = 1;
    private const float fastAccelation = 30;

    private readonly float downInterval = 1f;
    private readonly float repeatInterval = 0.1f;
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
        tetriminoManager.InitManager(new Vector3[tetriminoArraySize]
        { 
            grid.GetSpawnPoint, 
            nextTetriminoGroup.GetChild(0).position, 
            nextTetriminoGroup.GetChild(1).position, 
            nextTetriminoGroup.GetChild(2).position
        },
        holdTransform.position, 
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

        isHolding = false;
        isStop = true;
        level = 1;
        score = 0;
        downTime = 0;
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
            if (Input.GetKeyDown(KeyCode.Space))
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
                    tetriminoManager.InitTetriminos();
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
            Falling();
            Moving();

            //Holding
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isHolding)
                isHolding = tetriminoManager.HoldCurrentTetrimino();
            
            //Rotating
            if (Input.GetKeyDown(KeyCode.UpArrow))
                RotateTetrimino();

            //Hard drop
            if (Input.GetKeyDown(KeyCode.Space))
            {
                downTime = downInterval;
                HardDrop();
                sound.PlaySFX(SFX.HardDrop);
                isHolding = true;
            }

            UpdateGhostPosition();
        }
    }

    private void Falling()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            accelation = fastAccelation;

        if (Input.GetKeyUp(KeyCode.DownArrow))
            accelation = normalAccelation;

        if (downTime < (downInterval - (level * 0.1f)))
            downTime += Time.deltaTime * accelation;
        else
        {
            MoveTetrimino(Vector2Int.down);
            downTime = 0;
        }
    }

    private void Moving()
    {
        Moving(KeyCode.LeftArrow, Vector2Int.left);
        Moving(KeyCode.RightArrow, Vector2Int.right);
    }

    private void Moving(KeyCode code, Vector2Int direction)
    {
        if (Input.GetKeyDown(code))
        {
            MoveTetrimino(direction);
            repeatTime = 0;
        }
        if (Input.GetKey(code))
        {
            if (repeatInterval > repeatTime)
                repeatTime += Time.deltaTime;
            else
            {
                MoveTetrimino(direction);
                repeatTime = 0;
            }
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
                isHolding = false;
                currentTetrimino.Release();
            }
        }
    }

    private void RotateTetrimino()
    {
        if (grid.IsRotateValidation(currentTetrimino.NextRotateArray(), currentTetrimino.PositionInGrid))
        {//회전 가능 한지 체크
            currentTetrimino.Rotate();

            //벽을 뚫으면 나간 만큼 반대로 밀음
            var kicked = grid.WallKick(currentTetrimino);
            currentTetrimino.SetPosition(kicked);

            UpdateGhostPosition(true);

            if (!grid.IsRotateValidation(currentTetrimino.CurrentArray, currentTetrimino.PositionInGrid))
            {// 회전 후 밀어진 상황에서 기존 블럭과 충돌 시 원복
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
