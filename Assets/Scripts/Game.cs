using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeReference]
    Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeReference]
    GameBoard board = default;
    [SerializeField]
    GameTileContentFactory tileContentFactory = default;
    [SerializeField]
    EnemyFactory enemyFactory = default;

    [SerializeField, Range(0.1f, 10f)]
    float spawnSpeed = 1.0f;

    float spawnProgress;
    EnemyCollection enemies = new EnemyCollection();
    private void Awake()
    {
        board.Initialize(boardSize, tileContentFactory);
        board.ShowGrid = true;
    }

    private void OnValidate()
    {
        if (boardSize.x < 2)
        {   
            boardSize.x = 2;
        }
        if (boardSize.y < 2)
        {
            boardSize.y = 2;
        }
    }

    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    private void Update()
    {
        spawnProgress += spawnSpeed * Time.deltaTime;
        while(spawnProgress >= 1f)
        {
            spawnProgress -= 1f;
            SpawnEnemy();
        }
        if (Input.GetMouseButtonDown(0))
        {
            HandelTouch(0);
        }
        else if(Input.GetMouseButtonDown(1))
        {
            HandelTouch(1);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            board.ShowPath = !board.ShowPath;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            board.ShowGrid = !board.ShowGrid;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            board.ShowGrid = !board.ShowGrid;
        }
        enemies.GameUpdate();
        Physics.SyncTransforms();
        board.GameUpdate();
    }

    void HandelTouch(int operate)
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            switch (operate)
            {
                case 0:
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        board.ToggleSpawnPoint(tile);
                    }
                    else
                    {
                        board.ToggleDestination(tile);
                    }
                    break;
                case 1:
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        board.ToggleDestination(tile);
                    }
                    else
                    {
                        board.ToggleWall(tile);
                    }
                    board.ToggleTower(tile);

                    break;
            }
        }
    }

    void SpawnEnemy()
    {
        GameTile spwanPoint = board.GetSpawnPoint(Random.Range(0, board.SpwanPointCount));
        Enemy enemy = enemyFactory.Get();
        enemy.SpawnOn(spwanPoint);
        enemies.Add(enemy);
    }
}
