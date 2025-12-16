using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck;
using UnityEngine;

public class GlassDome : BulletTarget
{
    [SerializeField] Item[] items;
    [SerializeField] int hp;
    [SerializeField] GameObject[] glass;
    [SerializeField] Vector2 brokeForce;
    int curHP;
    int brokePart;

    protected override void Awake()
    {
        base.Awake();
        curHP = hp;
    }

    void Start() => items.ForEach(x => x.SetKinematic());

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        if (curHP <= 0) return;
        curHP -= damage;

        if (curHP <= 0) Broke();
        else if ((1 - (float)curHP / hp) * 3 > brokePart)
        {
            List<GameObject> glass = new(this.glass);
            for (int i = 0; i < 4; i++)
            {
                int index = Random.Range(0, glass.Count);
                Transform glassTransform = glass[index].transform;
                glassTransform.position += (glassTransform.position - thisTransform.position).normalized * Random.Range(0.015f, 0.03f);
                glass.RemoveAt(index);
            }
            brokePart++;
            SoundHolder.Default.PlayFromSoundPack("Glass Crack", thisTransform);
        }
    }

    void Broke()
    {
        Collect();
        glass.ForEach(x =>
        {
            Rigidbody rigidbody = x.AddComponent<Rigidbody>();
            rigidbody.velocity += (x.transform.position - thisTransform.position).normalized * Random.Range(brokeForce.x, brokeForce.y);
            x.transform.DOScale(0, 0.33f).SetDelay(2).OnComplete(() => Destroy(x));
        });
        SoundHolder.Default.PlayFromSoundPack("Glass Broke", thisTransform);
        items.ForEach(x => x.SetKinematic(false));
    }
}
