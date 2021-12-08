using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField]
    Enemy prefab = default;

    [SerializeField, FloatRangeSlider(.5f, 2f)]
    FloatRange scale = new FloatRange(1f);

    [SerializeField, FloatRangeSlider(-.4f, .4f)]
    FloatRange pathOffset = new FloatRange(0f);

    [SerializeField, FloatRangeSlider(0.2f, 5f)]
    FloatRange speed = new FloatRange(1f);
    public Enemy Get()
    {
        Enemy instance = CreatGameObjectInstance(prefab);
        instance.OriginFactory = this;
        instance.Initialize(scale.RandomValueInRange, speed.RandomValueInRange, pathOffset.RandomValueInRange);
        return instance;
    }


}
