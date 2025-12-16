using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AgressiveDrone : Turret
{
    [SerializeField] GameObject gun;

    protected override void Death()
    {
        base.Death();
        gameObject.layer = 5;
        thisRenderer.AddComponent<Rigidbody>();
        gun.AddComponent<Rigidbody>();
        gun.transform.parent = Level.Instance.thisTransform;
    }
}
