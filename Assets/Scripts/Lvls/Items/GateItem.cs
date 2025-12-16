using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateItem : Item
{
    public override void WeaponHit(Collider collider)
    {
        base.WeaponHit(collider);
        thisTransform.GetChild(0).GetComponent<Gate>().WeaponHit(collider);
    }
}
