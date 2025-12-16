using UnityEngine;

public class WeaponItem : Item
{
    [SerializeField] int lvl;
    bool collect;

    public override void WeaponHit(Collider collider)
    {
        if (collect) return;
        collect = true;
        
        Weapon weapon = PlayerController.Instance.InstantiateWeapon(PlayerController.Instance.Weapons[0]);
        weapon.SetSkin(GameData.Default.weapons[lvl]);
        weapon.State = WeaponState.Shoot;
        
        base.WeaponHit(collider);
        Destroy(gameObject);
    }
}