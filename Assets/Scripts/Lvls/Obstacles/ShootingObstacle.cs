using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingObstacle : MonoBehaviour
{
    [NonSerialized] public Transform thisTransform;
    
    [Header("Params")]
    [SerializeField] int damage;
    [SerializeField] float fireRate, bulletSpeed;
    [SerializeField] float bulletLifeTime = 7;
    
    [Space, Header("Refs")]
    [SerializeField] EnemyBullet bullet;
    [SerializeField] Vector3 gunPoint;
    float shootT;

    private void Awake()
    {
        thisTransform = transform;
    }

    void Update()
    {
        shootT += Time.deltaTime * fireRate;
        if (shootT > 1)
        {
            shootT -= 1;
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(bullet, thisTransform.TransformPoint(gunPoint), Quaternion.LookRotation(thisTransform.forward), thisTransform).Shoot(thisTransform.forward, bulletSpeed, damage, bulletLifeTime);
    }
}
