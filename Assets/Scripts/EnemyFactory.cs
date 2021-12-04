using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField]
    Enemy prefab = default;
    public Enemy Get()
    {
        Enemy instance = CreatGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }

}
