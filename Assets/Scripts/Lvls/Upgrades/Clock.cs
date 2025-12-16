using System;
using DG.Tweening;
using Dreamteck;
using UnityEngine;

public class Clock : BulletTarget, IWeaponTarget
{
    private enum RotatedAxis
    {
        X, Y, Z
    }

    [SerializeField] private RotatedAxis rotatedAxis;
    [SerializeField] float speed, boostSpeed;
    [SerializeField] Transform arrow;
    [SerializeField] ParticleSystem[] particles;
    Tween boostTween;
    Vector3 startScale;

    protected override void Awake()
    {
        base.Awake();
        startScale = thisTransform.localScale;
    }

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        ActionOnBulletHit(damage, multiplyDamage);
        if (!boostTween.IsActive()) particles.ForEach(x => x.Play());
        boostTween.Kill();
        thisTransform.localScale = startScale;
        boostTween = thisTransform.DOScale(startScale * 1.15f, 0.2f).SetLoops(6, LoopType.Yoyo).OnComplete(EndBoost);
    }

    protected virtual void ActionOnBulletHit(int damage, int multiplyDamage = 1) =>
        PlayerController.Instance.Upgrade(UpgradeType.Years, 5 * multiplyDamage, false);

    void EndBoost()
    {
        particles.ForEach(x => x.Stop());
    }

    public void WeaponHit(Collider collider) => Collect();

    protected override void Collect()
    {
        if (state == State.Collect) return;
        base.Collect();

        boostTween.Kill();
        thisTransform.DOScale(0, 0.33f).OnComplete(() => Destroy(gameObject));
    }

    void Update()
    {
        arrow.localRotation = rotatedAxis switch
        {
            RotatedAxis.X => Quaternion.Euler(Time.time * (boostTween.IsActive() ? boostSpeed : speed), 0, 0),
            RotatedAxis.Y => Quaternion.Euler(0, Time.time * (boostTween.IsActive() ? boostSpeed : speed), 0),
            RotatedAxis.Z => Quaternion.Euler(0, 0, Time.time * (boostTween.IsActive() ? boostSpeed : speed)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
