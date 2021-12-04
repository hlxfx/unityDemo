using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyFactory originFactory;
    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress;
    Direction direction;
    DirectionChange directionChange;
    float directionAngelFrom, directionAngelTo;
    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            originFactory = value;
        }
    }

    public void SpawnOn(GameTile tile)
    {
        tileFrom = tile;
        tileTo = tile.NextTilePath;
        progress = 0f;
        PrepareInfo();
    }

    void PrepareInfo()
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.Pathdirection;
        directionChange = DirectionChange.None;
        directionAngelFrom = directionAngelTo = direction.GetAngel();
        transform.localRotation = tileFrom.Pathdirection.GetRotation();
    }

    void PrepareNextState()
    {
        positionFrom = positionTo;
        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChange(tileFrom.Pathdirection);
        direction = tileFrom.Pathdirection;
        directionAngelFrom = directionAngelTo;
        switch (directionChange)
        {
            case DirectionChange.None:
                PrepareFoward();
                break;
            case DirectionChange.TurnRight:
                PrepareTurnRight();
                break;
            case DirectionChange.TurnLeft:
                PrepareTurnLeft();
                break;
            default:
                PrepareTurnAround();
                break;

        }
    }

    void PrepareFoward()
    {
        transform.localRotation = tileFrom.Pathdirection.GetRotation();
        directionAngelTo = direction.GetAngel();
    }
    void PrepareTurnRight()
    {
        directionAngelTo = directionAngelFrom + 90f;
    }
    void PrepareTurnLeft()
    {
        directionAngelTo = directionAngelFrom - 90f;
    }
    void PrepareTurnAround()
    {
        directionAngelTo = directionAngelFrom + 180f;
    }

    public bool GameUpdate()
    {
        transform.localPosition += Vector3.forward * Time.deltaTime;
        progress += Time.deltaTime;
        while(progress >= 1f)
        {
            tileFrom = tileTo;
            tileTo = tileTo.NextTilePath;
            if(tileTo == null)
            {
                originFactory.ReClaim(gameObject);
                return false;
            }
            PrepareNextState();
            progress -= 1f;
        }
        transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        if(directionChange != DirectionChange.None)
        {

        }
        return true;
    }
}
