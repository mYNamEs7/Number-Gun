using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TargetObstacle : HPObstacle
{
    [SerializeField] List<GameObject> parts;
    int partCount;

    protected override void Awake()
    {
        base.Awake();
        partCount = parts.Count;
    }

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        base.BulletHit(damage);
        while ((float)curHP / hp < (float)parts.Count / partCount && parts.Count > 0)
        {
            GameObject part = parts[0];
            Rigidbody rigidbody = part.AddComponent<Rigidbody>();
            rigidbody.velocity += new Vector3(Random.Range(-1f, 1f), Random.Range(0.25f, 1f), 0) * 5;
            Transform transform = part.transform;
            transform.parent = Level.Instance.thisTransform;
            transform.DOScale(0, 0.33f).SetDelay(0.5f).OnComplete(() => Destroy(part));
            parts.RemoveAt(0);
        }
    }
}
