using UnityEngine;

public class TeleportBossGate : TeleportGate
{
    [SerializeField] Tank tank;

    public override void WeaponHit(Collider collider)
    {
        base.WeaponHit(collider);
        tank.Enable();
    }
}
