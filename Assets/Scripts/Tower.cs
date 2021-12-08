using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10f)]
    float targetingRange = 1.5f;
    [SerializeField]
    Transform turret = default;


    static Collider[] targetBuffer = new Collider[1];
    TargetPoint target;

    public override void GameUpdate()
    {
        base.GameUpdate();
        if(TargetLock() || AcquireTarget())
        {
            Shoot();
        }
    }
           
    public void Shoot()
    {
        Vector3 point = target.Position;
        turret.LookAt(point);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        Debug.Log(position);
        Debug.Log(transform.position);
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
        if(target != null)
        {
            Gizmos.DrawLine(position, target.Position);
        }
    }

    bool AcquireTarget()
    {
        Vector3 a = transform.localPosition;
        Vector3 b = a;
        b.y += 3f;
        int hits = Physics.OverlapCapsuleNonAlloc(a, b, targetingRange,targetBuffer, 1 << 9);  // Ô²ÖùÌåÉäÏß¼ì²â
        if (hits > 0)
        {
            target = targetBuffer[0].GetComponent<TargetPoint>();
            Debug.Assert(target != null, "targeted non-enemy", targetBuffer[0]);
            return true;
        }
        target = null;
        return false;
    }

    bool TargetLock()
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
