using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyFactory originFactory;
    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress, progressFator; //控制移动倍速
    Direction direction;
    DirectionChange directionChange;
    float directionAngelFrom, directionAngelTo;

    [SerializeField]
    Transform modle;
   
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
        PrepareIntro();
    }

    void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.Pathdirection;
        directionChange = DirectionChange.None;
        directionAngelFrom = directionAngelTo = direction.GetAngel();
        transform.localRotation = tileFrom.Pathdirection.GetRotation();
        progressFator = 2f; //因为只走了一半
    }

    void PrepareOutro()
    {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngelTo = direction.GetAngel();
        modle.localPosition = Vector3.zero;
        transform.localRotation = direction.GetRotation();
        progressFator = 2f;
    }

    void PrepareNextState()
    {
        tileFrom = tileTo;
        tileTo = tileTo.NextTilePath;
        positionFrom = positionTo;
        if(tileTo == null)
        {
            PrepareOutro();
            return;
        }
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
        progressFator = 1f;
        transform.localRotation = tileFrom.Pathdirection.GetRotation();
        directionAngelTo = direction.GetAngel();
        modle.localPosition = Vector3.zero;
    }
    void PrepareTurnRight()
    {
        progressFator =  1f / (Mathf.PI * 0.25f);
        directionAngelTo = directionAngelFrom + 90f;
        modle.localPosition = new Vector3(-.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
    }
    void PrepareTurnLeft()
    {
        progressFator = 1f / (Mathf.PI * 0.25f);
        directionAngelTo = directionAngelFrom - 90f;
        modle.localPosition = new Vector3(.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
    }
    void PrepareTurnAround()
    {
        progressFator = 2f;
        directionAngelTo = directionAngelFrom + 180f;
        modle.localPosition = Vector3.zero;
        transform.localPosition = positionFrom;
    }

    public bool GameUpdate()
    {
        progress += Time.deltaTime * progressFator;
        while(progress >= 1f)
        {
            //tileFrom = tileTo;
            //tileTo = tileTo.NextTilePath;
            if(tileTo == null)
            {
                originFactory.ReClaim(gameObject);
                return false;
            }
            progress = (progress - 1f) / progressFator;
            PrepareNextState();
            progress *= progressFator;
        }
        //transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        //当方向发生变化时，我们绝对不能在Enemy.GameUpdate中完全插入位置，因为移动是通过旋转来完成的。
        if (directionChange == DirectionChange.None)
        {
            transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        }
        else
        {
            float angel = Mathf.LerpUnclamped(directionAngelFrom, directionAngelTo, progress);
            transform.localRotation = Quaternion.Euler(0f, angel, 0f);
        }
        return true;
    }


    public void Initialize(float scale)
    {
        modle.localScale = new Vector3(scale, scale, scale);
    }
}
