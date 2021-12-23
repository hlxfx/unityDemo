using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{
    public override TowerType TowerType => TowerType.Laser;

    [SerializeField]
    Transform turret = default, laserBeam = default;
    [SerializeField, Range(1f, 100f)]
    float damagePerSecond = 10f;


    Vector3 laserBeamScale;
    TargetPoint target;
    private void Awake()
    {
        laserBeamScale = laserBeam.localScale;
    }

    public override void GameUpdate()
    {
        base.GameUpdate();
        if (TargetLock(ref target) || AcquireTarget(out target))
        {
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
        }
    }

    public void Shoot()
    {
        Vector3 point = target.Position;
        turret.LookAt(point);
        laserBeam.localRotation = turret.localRotation;
        float d = Vector3.Distance(turret.position, point);
        laserBeamScale.z = d;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition = turret.localPosition + .5f * d * laserBeam.forward;
        target.enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }
}
