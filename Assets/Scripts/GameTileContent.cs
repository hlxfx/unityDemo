using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameTileContentType
{
    Empty, Destination, Wall, SpawnPoint, Tower
}

public class GameTileContent : MonoBehaviour
{
    [SerializeField]
    GameTileContentType type = default;
    public GameTileContentType Type => type;

    public bool BlocksPath => Type == GameTileContentType.Wall || Type == GameTileContentType.Tower;

    GameTileContentFactory originFactory;
    public GameTileContentFactory OriginFactory
    {
        get => originFactory;
        set
        {
            originFactory = value;
        }
    }

    public void Recycle()
    {
        originFactory.ReClaim(gameObject);
    }


    public virtual void GameUpdate()
    {

    }

    private void OnDestroy()
    {
        Recycle();
    }
}
