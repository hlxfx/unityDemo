using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameTileContentType
{
    Empty, Destination, Wall, SpawnPoint
}

public class GameTileContent : MonoBehaviour
{
    [SerializeField]
    GameTileContentType type = default;
    public GameTileContentType Type => type;

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
}
