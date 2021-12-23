using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    Laser, Mortar,
}

public abstract class Tower : GameTileContent
{
    public abstract TowerType TowerType {get; }

    [SerializeField, Range(1.5f, 10f)]
    float targetingRange = 1.5f;
    const int enemyLayerMask = 1 << 9;

    static Collider[] targetBuffer = new Collider[10];
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        Debug.Log(position);
        Debug.Log(transform.position);
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
        /*if(target != null)
        {
            Gizmos.DrawLine(position, target.Position);
        }*/
    }

    protected bool AcquireTarget(out TargetPoint target)
    {
        Vector3 a = transform.localPosition;
        Vector3 b = a;
        b.y += 3f;
        int hits = Physics.OverlapCapsuleNonAlloc(a, b, targetingRange,targetBuffer, enemyLayerMask);  // Ô²ÖùÌåÉäÏß¼ì²â
        if (hits > 0)
        {
            target = targetBuffer[Random.Range(0,hits)].GetComponent<TargetPoint>();
            Debug.Assert(target != null, "targeted non-enemy", targetBuffer[0]);
            return true;
        }
        target = null;
        return false;
    }

    protected bool TargetLock(ref TargetPoint target)
    {
        if (target == null)
        {
            return false;
        }
        Vector3 a = transform.localPosition;
        Vector3 b = target.Position;
        float x = a.x - b.x;
        float z = a.z - b.z;
        float r = targetingRange + 0.125f * target.enemy.Scale;
        if (x * x + z * z > r * r)
        {
            target = null;
            return false;
        }
        return true;
    }
}
