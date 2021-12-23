using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyFactory originFactory;
    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress, progressFactor; //敌人移动进度条及倍速
    Direction direction;
    DirectionChange directionChange;
    float directionAngelFrom, directionAngelTo;
    float pathOffset, speed;
    float Health { get; set; }

    [SerializeField]
    Transform model = default;

    public float Scale
    {
        get;
        private set;
    }
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

    public bool GameUpdate()
    {
        if(Health < 0)
        {
            originFactory.ReClaim(gameObject);
            return false;
        }
        progress += Time.deltaTime * progressFactor;
        while(progress >= 1f)
        {
            //tileFrom = tileTo;
            //tileTo = tileTo.NextTilePath;
            if(tileTo == null)
            {
                originFactory.ReClaim(gameObject);
                return false;
            }
            progress = (progress - 1f) / progressFactor;
            PrepareNextState();
            progress *= progressFactor;
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

    public void Initialize(float scale, float speed, float pathOffset)
    {
        model.localScale = new Vector3(scale, scale, scale);
        Scale = scale;
        this.pathOffset = pathOffset;
        this.speed = speed;
        Health = 100f * scale;
    }

    public void ApplyDamage(float damage)
    {
        Health -= damage;
    }

    void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.Pathdirection;
        directionChange = DirectionChange.None;
        directionAngelFrom = directionAngelTo = direction.GetAngel();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }

    void PrepareOutro()
    {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngelTo = direction.GetAngel();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }

    void PrepareNextState()
    {
        tileFrom = tileTo;
        tileTo = tileTo.NextTilePath;
        positionFrom = positionTo;
        if (tileTo == null)
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
        transform.localRotation = direction.GetRotation();
        directionAngelTo = direction.GetAngel();
        model.localPosition = new Vector3(pathOffset, 0f);
        progressFactor = speed;
    }
    void PrepareTurnRight()
    {
        directionAngelTo = directionAngelFrom + 90f;
        model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
    }
    void PrepareTurnLeft()
    {
        directionAngelTo = directionAngelFrom - 90f;
        model.localPosition = new Vector3(pathOffset + 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f + pathOffset));
    }

    void PrepareTurnAround()
    {
        directionAngelTo = directionAngelFrom + (pathOffset < 0f ? 180f : -180f); ;
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localPosition = positionFrom;
        progressFactor = speed / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
    }
}
