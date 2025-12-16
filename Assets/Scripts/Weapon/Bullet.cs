using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    const float DistanceBetweenChilds = 0.5f;

    BulletData data;
    Vector3 dir;
    int damage, multiplyDamage = 1;

    [SerializeField] TrailRenderer trailRenderer;
    [NonSerialized] public GameObject thisGameObject;
    [NonSerialized] public Transform thisTransform;
    float startZ;
    Vector3 oldPos;
    bool shoot, checkCollision;

    ObjectPool<Bullet> pool;
    Bullet[] childs;

    private List<BulletTarget> ignoredObjects;

    public void Init(ObjectPool<Bullet> pool, List<BulletTarget> ignoredObjects = null)
    {
        this.pool = pool;
        thisGameObject = gameObject;
        thisTransform = transform;
        trailRenderer.enabled = false;
        this.ignoredObjects = ignoredObjects;
    }

    public void Shoot(Vector3 dir, BulletData data, int damage, int multiplyDamage = 1, bool checkCollision = true, Bullet[] childs = null)
    {
        shoot = true;
        this.checkCollision = checkCollision;
        this.dir = dir;
        this.data = data;
        this.damage = damage;
        this.multiplyDamage = multiplyDamage;
        startZ = thisTransform.localPosition.z;
        thisTransform.localScale = Vector3.one * data.size;
        oldPos = thisTransform.position;

        if (childs != null)
        {
            thisTransform.localPosition += dir * (childs.Length * DistanceBetweenChilds);
            this.childs = childs;
            for (int i = 0; i < childs.Length; i++)
            {
                Transform transform = childs[i].thisTransform;
                transform.localScale = Vector3.one * data.size;
                transform.position = thisTransform.position - dir * ((i + 1) * DistanceBetweenChilds);
            }
            childs[^1].trailRenderer.enabled = true;
        }
        else trailRenderer.enabled = true;
    }

    void FixedUpdate()
    {
        if (!shoot) return;

        thisTransform.localPosition += dir * (Time.deltaTime * data.speed);
        if (checkCollision)
        {
            for (int i = 0; i < PhysicManager.BulletTargets.Count; i++)
                if ((ignoredObjects == null || !ignoredObjects.Contains(PhysicManager.BulletTargets[i])) && PhysicManager.BulletTargets[i].CheckCollision(oldPos, thisTransform.localPosition))
                {
                    Hit(PhysicManager.BulletTargets[i]);
                    break;
                }
        }

        if (childs != null) for (int i = 0; i < childs.Length; i++) childs[i].thisTransform.position = thisTransform.position - dir * ((i + 1) * DistanceBetweenChilds);
        if (thisTransform.localPosition.z > startZ + data.range) Destroy();
        oldPos = thisTransform.position;
    }

    void Hit(BulletTarget bulletTarget)
    {
        bulletTarget.BulletHit(damage, multiplyDamage);
        if (bulletTarget as Card) Instantiate(GameData.Default.bulletDestroyParticles, thisTransform.position, Quaternion.LookRotation(-dir));
        SoundHolder.Default.PlayHit();
        Destroy();
    }

    void Destroy()
    {
        shoot = false;

        if (childs != null)
        {
            for (int i = 0; i < childs.Length; i++) childs[i].Destroy();
            childs = null;
        }

        trailRenderer.Clear();
        trailRenderer.enabled = false;
        if (pool == null) Destroy(gameObject); else pool.Release(this);
    }
}

[Serializable]
public class BulletData
{
    public float speed;
    public float range;
    public float size;
    public float fireRate;
}