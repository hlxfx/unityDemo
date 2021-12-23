using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTower : Tower
{
    public override TowerType TowerType => TowerType.Mortar;

    [SerializeField, Range(0.5f, 2f)]
    float shotsPerSecond = 1f;
    [SerializeField]
    Transform mortar = default;

    float launchSpeed;
    float x = targetingRange + 0.25001f;
    float s = launchSpeed;

    void Awake()
    {
        OnValidate();
    }

    void OnValidate()
    {
        float x = targetingRange;
        float y = -mortar.position.y;
        launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
    }

    public override void GameUpdate()
    {
        base.GameUpdate();
        Launch(new Vector3(3f, 0f, 0f));
        Launch(new Vector3(0f, 0f, 1f));
        Launch(new Vector3(1f, 0f, 1f));
        Launch(new Vector3(3f, 0f, 1f));
    }

    public void Launch(Vector3 offset)
    {
        Vector3 launchPoint = mortar.position;
        Vector3 targetPoint = launchPoint + offset;
        /*
        Debug.DrawLine(launchPoint, new Vector3(targetPoint.x, 0.01f, targetPoint.z), Color.yellow);
        Debug.DrawLine(
            new Vector3(launchPoint.x, 0.01f, launchPoint.z),
            new Vector3(targetPoint.x, 0.01f, targetPoint.z),Color.white);
        Vector3 prev = launchPoint, next;
        float sx = 
        float v = 5;
        for (int i = 1; i <= 10; i++)
        {
            float t = i / 10f;
            float dx = s *  t;
            float dy = s *  t - 0.5f * g * t * t;
            next = launchPoint + new Vector3(dir.x * dx, dy, dir.y * dx);
            Debug.DrawLine(prev, next, Color.blue);
            prev = next;
        }
        */
        targetPoint.y = 0f;
        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;
        float x = dir.magnitude;
        float y = -launchPoint.y;
        dir /= x;

        float g = 9.81f;
        float s = 4f;
        float s2 = s * s;

        float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        Debug.Assert(r >= 0f, "Launch velocity insufficient for range!");
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        float sinTheta = cosTheta * tanTheta;

        Vector3 prev = launchPoint, next;
        for (int i = 1; i <= 10; i++)
        {
            float t = i / 10f;
            float dx = s * cosTheta * t;
            float dy = s * sinTheta * t - 0.5f * g * t * t;
            next = launchPoint + new Vector3(dir.x * dx, dy, dir.y * dx);
            Debug.DrawLine(prev, next, Color.blue);
            prev = next;
        }

        Debug.DrawLine(launchPoint, targetPoint, Color.yellow);
        Debug.DrawLine(
            new Vector3(launchPoint.x, 0.01f, launchPoint.z),
            new Vector3(
                launchPoint.x + dir.x * x, 0.01f, launchPoint.z + dir.y * x
            ),
            Color.white
        );
    }

}
