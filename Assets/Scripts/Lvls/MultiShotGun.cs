using DG.Tweening;
using UnityEngine;

public class MultiShotGun : ScaleBulletTarget, IWeaponTarget
{
    [SerializeField] Transform[] gunPoints;

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        base.BulletHit(damage);

        Weapon weapon = PlayerController.Instance.Weapons[0];
        for (int i = 0; i < gunPoints.Length; i++)
        {
            Bullet bullet = weapon.bulletsPool.Get();
            bullet.thisTransform.position = gunPoints[i].position;
            bullet.Shoot(gunPoints[i].parent.TransformDirection(gunPoints[i].localPosition).normalized, weapon.bulletData, damage);
        }
    }

    public void WeaponHit(Collider collider) => Collect();

    void Update()
    {
        if (state == State.Collect) return;
        if (thisTransform.position.z < PlayerController.Instance.thisTransform.position.z) Collect();
    }

    protected override void Collect()
    {
        if (state == State.Collect) return;
        base.Collect();
        thisTransform.DOScale(0, 0.33f).OnComplete(() => Destroy(gameObject));
    }
}
