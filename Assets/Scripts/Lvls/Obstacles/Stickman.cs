using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Stickman : BulletTarget
{
    [SerializeField] int hp;
    [SerializeField] TMP_Text hpTxt;
    [SerializeField] Renderer thisRenderer;
    Material material;
    Transform hpTransform;
    Tween hpScaleTween;

    protected override void Awake()
    {
        base.Awake();
        material = new Material(thisRenderer.sharedMaterial);
        thisRenderer.sharedMaterial = material;
        hpTransform = hpTxt.transform;

        UpdateHP();
    }

    void UpdateHP() => hpTxt.text = hp.ToString();

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        hp -= damage;
        UpdateHP();
        hpScaleTween.Kill(true);
        hpScaleTween = hpTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 0);
        if (hp <= 0) Death();
    }

    void Death()
    {
        Collect();
        Destroy(hpTxt.gameObject);
        thisRenderer.gameObject.AddComponent<Rigidbody>();
        material.SetTexture("_BaseMap", null);
        material.SetColor("_BaseColor", Color.gray);
        thisRenderer.gameObject.layer = 5;
    }
}
