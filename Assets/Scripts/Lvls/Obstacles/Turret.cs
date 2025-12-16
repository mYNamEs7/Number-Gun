using DG.Tweening;
using TMPro;
using UnityEngine;

public class Turret : ScaleBulletTarget
{
    [SerializeField] int hp;
    [SerializeField] int damage;
    [SerializeField] float fireRate, bulletSpeed;
    [SerializeField] float bulletLifeTime = 7;
    [SerializeField] Vector3 gunPoint;

    [Space, Header("Refs")]
    [SerializeField] protected Renderer thisRenderer;
    [SerializeField] GameObject hpTag;
    [SerializeField] TextMeshPro hpTxt;
    [SerializeField] EnemyBullet bullet;
    Material material;
    bool death;
    float shootT;

    protected override void Awake()
    {
        base.Awake();
        material = new(thisRenderer.sharedMaterial);
        thisRenderer.sharedMaterial = material;
        UpdateHP();
    }

    void UpdateHP() => hpTxt.text = hp.ToString();

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        if (death) return;
        hp -= damage;
        UpdateHP();

        if (hp <= 0) Death();
    }

    protected virtual void Death()
    {
        death = true;
        material.SetTexture("_BaseMap", null);
        material.SetColor("_BaseColor", Color.gray);
        Destroy(hpTag);
    }

    void Update()
    {
        if (death) return;
        shootT += Time.deltaTime * fireRate;
        if (shootT > 1)
        {
            shootT -= 1;
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(bullet, thisTransform.TransformPoint(gunPoint), Quaternion.LookRotation(-thisTransform.forward), thisTransform).Shoot(-thisTransform.forward, bulletSpeed, damage, bulletLifeTime);
    }
}