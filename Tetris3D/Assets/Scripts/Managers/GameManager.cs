using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform nextTetriminoGroup;

    private GameStatus status;
    private Grid grid;
    private TetriminoSpawner spawner;

    void Start()
    {
        status = GameStatus.Title;
        grid = new Grid();
        spawner = new TetriminoSpawner(new Vector3[4] {
            grid.GetSpawnPoint,
            nextTetriminoGroup.GetChild(0).position,
            nextTetriminoGroup.GetChild(1).position,
            nextTetriminoGroup.GetChild(2).position}, 1);
    }

    void Update()
    {
        if(status == GameStatus.Title)
        {
            if(Input.anyKeyDown)
            {
                SetScene(GameStatus.Lobby);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
            spawner.GetPool.Get();
    }

    void SetScene(GameStatus status)
    {
        switch(status)
        {
            case GameStatus.Lobby:
                Lobby();
                break;

            case GameStatus.Play:
                break;

            case GameStatus.Result:
                break;

            case GameStatus.Title:
            default:
                break;
        }
    }

    void Lobby()
    {

    }
}
