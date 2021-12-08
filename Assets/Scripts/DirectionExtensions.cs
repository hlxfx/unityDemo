using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North, East, South, West
}

public enum DirectionChange
{
    None, TurnRight, TurnLeft, TurnAround
}
public static class DirectionExtensions
{
    static Quaternion[] rotation =
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };


    public static Quaternion GetRotation(this Direction direction)
    {
        return rotation[(int)direction];
    }

    public static DirectionChange GetDirectionChange(this Direction current, Direction next)
    {
        if(current == next)
        {
            return DirectionChange.None;
        }
        else if (current + 1 == next || current - 3 == next)
        {
            return DirectionChange.TurnRight;
        }
        else if (current - 1 == next || current + 3 == next)
        {
            return DirectionChange.TurnLeft;
        }
        return DirectionChange.TurnAround;
    }

    public static float GetAngel(this Direction direction)
    {
        return (float)direction * 90f;
    }

    static Vector3[] halfVectors = {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f
    };

    public static Vector3 GetHalfVector(this Direction direction)
    {
        return halfVectors[(int)direction];
    }


    [System.Serializable]
    public struct FloatRange
    {

        [SerializeField]
        float min, max;

        public float Min => min;

        public float Max => max;

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

        public FloatRange(float min, float max)
        {
            this.min = min;
            this.max = max < min ? min : max;
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

}
