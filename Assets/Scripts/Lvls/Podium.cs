using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Podium : BulletTarget
{
    readonly static Dictionary<Language, string> KName = new Dictionary<Language, string> { { Language.RU, "Т" }, { Language.EN, "K" }, { Language.TR, "K" } };
    enum PodiumColor
    {
        Yellow, Orange, Red, Green, Cyan, Blue, Purple
    }

    [SerializeField, HideInInspector] protected int hp = 10;
    protected int curHP;
    [SerializeField, HideInInspector] PodiumColor color;
    [SerializeField, HideInInspector] protected Item item;

    [SerializeField] TextMeshPro hpTxt;
    [SerializeField] Renderer[] podiumRenderer;
    [SerializeField] private ParticleSystem destroyEffect;
    protected Transform hpTxtTransform;
    Tween hpScaleTween;

    protected override void Awake()
    {
        base.Awake();
        hpTxtTransform = hpTxt.transform;
        curHP = hp;
        if (GameData.LanguageEnable) UpdateHP();
        else GameData.OnLanguageUpdate += UpdateHP;
    }

    void Start() => item.SetKinematic();

    protected virtual void UpdateHP() => hpTxt.text = curHP < 1000 ? curHP.ToString() : Math.Round(curHP / 1000f, 1).ToString() + KName[GameData.Language];

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        curHP -= damage;
        if (curHP <= 0) Destroy();

        UpdateHP();
        hpScaleTween.Kill(true);
        hpScaleTween = hpTxtTransform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 0);
    }

    public override void Destroy()
    {
        item.Drop();
        hpScaleTween.Kill(true);
        
        destroyEffect.gameObject.SetActive(true);
        var effect = Instantiate(destroyEffect, destroyEffect.transform.position, destroyEffect.transform.rotation);
        effect.Play();
        
        base.Destroy();
    }
}