using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeReference]
    Transform Ground = default;
    [SerializeReference]
    GameTile TilePrefab = default;

    Queue<GameTile> searchFrontier = new Queue<GameTile>();

    GameTile[] tiles;

    Vector2Int size;

    public void Initialize (Vector2Int size)
    {
        this.size = size;
        Ground.localScale = new Vector3(size.x, size.y, 1f);
        tiles = new GameTile[size.x * size.y];

        Vector2 offset = new Vector2(
            (size.x - 1) * .5f, (size.y - 1) * .5f);

        for(int temp = 0, x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++, temp++)
            {
                tiles[temp] = Instantiate(TilePrefab);
                tiles[temp].transform.SetParent(transform, false);
                tiles[temp].transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
                tiles[temp].name = "(" + tiles[temp].transform.localPosition.x + "," + tiles[temp].transform.localPosition.z + ")";
                tiles[temp].IsAlternative = (y & 1) == 0;
                if ((x & 1) == 0)
                {
                    tiles[temp].IsAlternative = !tiles[temp].IsAlternative;
                }
                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tiles[temp - size.y], tiles[temp]);
                }
                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tiles[temp], tiles[temp - 1]);
                }
            }
        }

        FindPaths();
    }

    void FindPaths()
    {
        foreach (GameTile tile in tiles)
        {
            tile.ClearPath();
        }
        tiles[0].BecomeDestination();
        searchFrontier.Enqueue(tiles[0]);

        while (searchFrontier.Count > 0)
        {
            GameTile temp = searchFrontier.Dequeue();
            if (temp != null)
            {
                if (temp.IsAlternative)
                {
                    searchFrontier.Enqueue(temp.GrowPathNorth());
                    searchFrontier.Enqueue(temp.GrowPathEast());
                    searchFrontier.Enqueue(temp.GrowPathSouth());
                    searchFrontier.Enqueue(temp.GrowPathWest());
                }
                else
                {
                    searchFrontier.Enqueue(temp.GrowPathWest());
                    searchFrontier.Enqueue(temp.GrowPathSouth());
                    searchFrontier.Enqueue(temp.GrowPathEast());
                    searchFrontier.Enqueue(temp.GrowPathNorth());
                }
            }
        }

        foreach (GameTile tile in tiles)
        {
            tile.ShowPath();
        }
    }
}
