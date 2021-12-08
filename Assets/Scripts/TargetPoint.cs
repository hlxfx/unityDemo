using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    public Enemy enemy { get; private set; }
    public Vector3 Position => transform.position;

    private void Awake()
    {
        enemy = transform.root.GetComponent<Enemy>();
        Debug.Assert(enemy.gameObject.layer == 9, "Target point on wrong layer!", this);
    }
}
