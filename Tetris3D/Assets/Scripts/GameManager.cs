using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameStatus status;
    private Grid grid;

    void Start()
    {
        status = GameStatus.Title;
        grid = gameObject.AddComponent<Grid>();
        grid.GenerateGrid();
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
