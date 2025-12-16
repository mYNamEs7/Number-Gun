using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class BulletCloner : BulletTarget
{
    [SerializeField] private List<Transform> bulletTargets;
    
    [Header("Params")]
    [SerializeField] private float bulletSpeed = 7;
    [SerializeField] private int multiplier = 1;
    
    [Header("Refs")]
    [SerializeField] private List<Transform> bulletPoints;

    private ObjectPool<Bullet> bulletsPool;

    private BulletData bulletData;

    protected override void Awake()
    {
        base.Awake();
        bulletData = new()
        {
            speed = bulletSpeed,
            range = GameData.Default.fireRange * GameData.Default.GetUpgrade(UpgradeType.FireRange).CurValue + GameData.AddedFireRange,
            size = GameData.Default.bulletSize,
            fireRate = GameData.Default.fireRate * GameData.Default.GetUpgrade(UpgradeType.FireRate).CurValue + GameData.AddedFireRate
        };
        bulletsPool = NewBulletsPool();
    }

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        for (int i = 0; i < bulletPoints.Count; i++)
        {
            var curPoint = bulletPoints[i];
            var curTarget = bulletTargets.Count <= i ? bulletTargets.Last() : bulletTargets[i];
            if (!curTarget) curTarget = bulletTargets.Last(x => x);
            var dir = curTarget.position - curPoint.position;

            var bullet = bulletsPool.Get();
            bullet.thisTransform.position = curPoint.position;
            bullet.Shoot(dir - Vector3.up * dir.y, bulletData, damage * multiplier);
        }
    }

    ObjectPool<Bullet> NewBulletsPool() => new ObjectPool<Bullet>(() => NewBullet(),
        (obj) => obj.thisGameObject.SetActive(true), (obj) => obj.thisGameObject.SetActive(false),
        (obj) => Destroy(obj.thisGameObject), false, 10, 1000);

    Bullet NewBullet()
    {
        Bullet bullet = Instantiate(PlayerController.Instance.Weapons[0].data.bullet, Level.Instance.thisTransform);
        bullet.Init(bulletsPool, new List<BulletTarget> { this });
        return bullet;
    }
}
