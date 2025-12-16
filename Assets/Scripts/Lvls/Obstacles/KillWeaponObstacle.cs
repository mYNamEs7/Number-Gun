using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillWeaponObstacle : MonoBehaviour, IWeaponTarget
{
    public void WeaponHit(Collider collider)
    {
        if (collider.TryGetComponent(out Weapon weapon)) PlayerController.Instance.KillWeapon(weapon);
    }
}
