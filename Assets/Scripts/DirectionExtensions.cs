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


    static Vector3[] halfVector =
    {
        Vector3.forward *.5f,
        Vector3.right * .5f,
        Vector3.back * .5f,
        Vector3.left * .5f,
    };

    public static Vector3 GetHalfVector(this Direction direction)
    {
        return halfVector[(int)direction];
    }

}
