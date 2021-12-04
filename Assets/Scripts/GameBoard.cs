using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeReference]
    Transform Ground = default;
    [SerializeReference]
    GameTile TilePrefab = default;
    [SerializeField]
    Texture2D gridTexture = default;

    [SerializeField]
    bool showGrid;
    public bool ShowGrid
    {
        get => showGrid;
        set
        {
            showGrid = value;
            Material m = Ground.GetComponent<MeshRenderer>().material;
            if (showGrid)
            {
                m.mainTexture = gridTexture;
                print(m.GetTextureScale("_MainTex"));
                m.SetTextureScale("_MainTex", size);
            }
            else
            {
                m.mainTexture = null;
            }
        }
    }
    [SerializeField]
    bool showPath;
    public bool ShowPath
    {
        get => showPath;
        set
        {
            showPath = value;
            if (showPath)
            {
                foreach(GameTile tile in tiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach (GameTile tile in tiles)
                {
                    tile.HidePath();
                }
            }
        }
    }

    Queue<GameTile> searchFrontier = new Queue<GameTile>();
    List<GameTile> spawnPoint = new List<GameTile>();
    GameTile[] tiles;
    Vector2Int size;
    GameTileContentFactory gameTileContentFactory;

    public int SpwanPointCount => spawnPoint.Count;

    public void Initialize (Vector2Int size ,GameTileContentFactory gameTileContentFactory)
    {
        this.size = size;
        this.gameTileContentFactory = gameTileContentFactory;
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
                tiles[temp].Content = gameTileContentFactory.Get(GameTileContentType.Empty);
            }
        }

        ToggleDestination(tiles[tiles.Length / 2]);
        ToggleSpawnPoint(tiles[0]);
    }

    bool FindPaths()
    {
        foreach (GameTile tile in tiles)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();
                searchFrontier.Enqueue(tile);
            }
            else
            {
               tile.ClearPath();
            }
        }

        if(searchFrontier.Count == 0)
        {
            return false;
        }

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
            if (!tile.HasPath)
            {
                return false;
            }
        }
        if (showPath)
        {
            foreach (GameTile tile in tiles)
            {
                tile.ShowPath();
            }
        }
        return true;
    }

    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);
            Debug.Log(x + "," + y);
            if (x >= 0 && x < size.x && y >= 0 && y < size.y)
                return tiles[x * size.y + y];
        }
        return null;
    }

    public void ToggleDestination(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = gameTileContentFactory.Get(GameTileContentType.Empty);
            if (!FindPaths())
            {
                tile.Content = gameTileContentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = gameTileContentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }
    }

    public void ToggleWall(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = gameTileContentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = gameTileContentFactory.Get(GameTileContentType.Wall);
            if (!FindPaths())
            {
                tile.Content = gameTileContentFactory.Get(GameTileContentType.Empty);
            }
            FindPaths();
        }
    }

    public void ToggleSpawnPoint(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.SpawnPoint)
        {
            if(spawnPoint.Count > 1)
            {
                spawnPoint.Remove(tile);
                tile.Content = gameTileContentFactory.Get(GameTileContentType.Empty);
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = gameTileContentFactory.Get(GameTileContentType.SpawnPoint);
            spawnPoint.Add(tile);
        }
    }

    public GameTile GetSpawnPoint(int index)
    {
        return spawnPoint[index];
    }

}
