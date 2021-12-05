using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameObjectFactory : ScriptableObject
{
    Scene scene;
    protected T CreatGameObjectInstance<T>(T prefab) where T : MonoBehaviour
    {
        if (!scene.isLoaded)
        {
            if (Application.isEditor)
            {
                scene = SceneManager.GetSceneByName(name);
                if (!scene.isLoaded)
                {
                    scene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                scene = SceneManager.CreateScene(name);
            }
        }
        T instance = Instantiate<T>(prefab);
        SceneManager.MoveGameObjectToScene(instance.gameObject, scene);
        return instance;
    }

    public void ReClaim(GameObject instance)
    {
        Destroy(instance);
    }
}

[System.Serializable]
public struct FloatRange
{
    [SerializeField]
    float min, max;

    public float Max => max;
    public float Min => min;

    public float RandomValueInRange
    {
        get
        {
            return Random.Range(min, max);
        }
    }

    public FloatRange(float value)
    {
        min = max = value;
    }

    public FloatRange(float min , float max)
    {
        this.min = min;
        this.max = max > min ? max : min;
    }
}

public class FloatRangeSliderAttribute : PropertyAttribute
{

    public float Min { get; private set; }

    public float Max { get; private set; }

    public FloatRangeSliderAttribute(float min, float max)
    {
        Min = min;
        Max = max < min ? min : max;
    }
}
