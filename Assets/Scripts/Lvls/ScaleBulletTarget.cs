using System;
using DG.Tweening;
using UnityEngine;

public abstract class ScaleBulletTarget : BulletTarget
{
    Tween scaleTween;

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        scaleTween.Kill(true);
        scaleTween = thisTransform.DOPunchScale(Vector3.one * GameData.Default.cardHitScale, GameData.Default.cardHitDuration, 0).SetEase(GameData.Default.cardHitEase);
    }

    protected override void Collect()
    {
        scaleTween.Kill(false);
        base.Collect();
    }
}