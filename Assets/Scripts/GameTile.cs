using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    public bool HasPath => distance != int.MaxValue;
    public bool IsAlternative { get; set; }


    [SerializeReference]
    Transform arrow = default;
    GameTile north, east, south, west, nextOnPath;
    int distance;
    static Quaternion
        northRotation = Quaternion.Euler(90f, 0f, 0f),
        eastRotation = Quaternion.Euler(90f, 90f, 0f),
        southRotation = Quaternion.Euler(90f, 180f, 0f),
        westRotation = Quaternion.Euler(90f, 270f, 0f);



    public static void MakeEastWestNeighbors(GameTile west, GameTile east)
    {
        west.east = east;
        east.west = west;
    }
    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        north.south = south;
        south.north = north;
    }

    public void ClearPath()
    {
        nextOnPath = null;
        distance = int.MaxValue;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
    }

    GameTile GrowPathTo(GameTile neighbor)
    {
        Debug.Assert(HasPath, "no Path!");
        if (!HasPath || neighbor == null || neighbor.HasPath) return null;
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        return neighbor;
    }

    public GameTile GrowPathNorth() => GrowPathTo(north);

    public GameTile GrowPathSouth() => GrowPathTo(south);


    public GameTile GrowPathEast() => GrowPathTo(east);


    public GameTile GrowPathWest() => GrowPathTo(west);

    public void ShowPath()
    {
        if(distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == east ? eastRotation :
            nextOnPath == south ? southRotation :
            nextOnPath == west ? westRotation :
            westRotation;

    }
}
