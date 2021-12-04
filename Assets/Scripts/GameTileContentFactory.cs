using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameTileContentFactory : GameObjectFactory
{
    Scene contentScene;
    [SerializeField]
    GameTileContent destinationPrefab = default;
    [SerializeField]
    GameTileContent emptyPrefab = default;
    [SerializeField]
    GameTileContent wallPrefab = default;
    [SerializeField]
    GameTileContent spawnPoint = default;

    GameTileContent Get(GameTileContent prefab)
    {
        GameTileContent instance = CreatGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }

    public GameTileContent Get(GameTileContentType type)
    {
        switch(type){
            case GameTileContentType.Destination:
                return Get(destinationPrefab);
            case GameTileContentType.Empty:
                return Get(emptyPrefab);
            case GameTileContentType.Wall:
                return Get(wallPrefab);
            case GameTileContentType.SpawnPoint:
                return Get(spawnPoint);
        }
        Debug.Assert(false, "UnSupported Type" + type);
        return null;
    }
}
