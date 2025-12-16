using DG.Tweening;
using TMPro;
using UnityEngine;

public class Gate : BulletTarget, IWeaponTarget
{
    readonly static Color RedColor = new Color(1, 0.15f, 0.15f);
    readonly static Color GreenColor = new Color(0.06f, 0.78f, 0.02f);

    [SerializeField] Gate secondGate;

    [Header("Data")]
    [SerializeField] UpgradeType upgradeType;
    [SerializeField] int startValue, maxValue;
    [SerializeField] float addValue = 1;
    int fill;
    bool isFill;
    float iconSize;
    Tween valueScaleTween, hitSoundTween;

    [Header("Refs")]
    [SerializeField] TextMeshPro addValueTxt;
    [SerializeField] TextMeshPro valueTxt;
    [SerializeField] SpriteRenderer icon;
    [SerializeField] SpriteRenderer spriteRenderer;
    Transform valueTxtTransform;

    protected override void Awake() => Init();

    public void Init()
    {
        if (upgradeType == UpgradeType.Years)
        {
            startValue += GameData.YearGateCapacity;
            maxValue += GameData.YearGateCapacity;
        }
        
        base.Awake();
        fill = startValue;

        if (icon) iconSize = icon.size.x;
        else
        {
            valueTxtTransform = valueTxt.transform;
            if (addValueTxt)
            {
                addValueTxt.text = (addValue < 0 ? "" : "+") + addValue;
                if (addValue < 0) addValueTxt.color = RedColor;
            }
        }
        UpdateValue();
    }

    void UpdateValue()
    {
        if (icon) icon.size = new Vector2(iconSize, Mathf.Lerp(0, iconSize, (float)fill / maxValue));
        else
        {
            valueTxt.text = (fill < 0 ? "" : "+") + fill;
            if (fill < 0)
            {
                valueTxt.color = RedColor;
                if (spriteRenderer) spriteRenderer.color = RedColor;
            }
            else
            {
                valueTxt.color = Color.white;
                if (spriteRenderer) spriteRenderer.color = GreenColor;
            }
        }
    }

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        float addValue = icon ? 1 : this.addValue;
        fill = Mathf.Min(fill + (int)addValue * multiplyDamage, maxValue);
        UpdateValue();

        if (!isFill && fill >= maxValue)
        {
            isFill = true;
            if (icon) valueScaleTween = icon.transform.DOPunchScale(Vector3.one * GameData.Default.cardHitScale, GameData.Default.cardHitDuration, 0).SetEase(GameData.Default.cardHitEase);
        }
        if (!hitSoundTween.IsActive())
        {
            if (!icon)
            {
                valueScaleTween.Kill(true);
                valueScaleTween = valueTxtTransform.DOPunchScale(Vector3.one * GameData.Default.cardHitScale, GameData.Default.cardHitDuration, 0).SetEase(GameData.Default.cardHitEase);
            }

            hitSoundTween = DOTween.Sequence().SetDelay(0.05f).OnComplete(() => { });
        }
    }

    public void WeaponHit(Collider collider)
    {
        if (TryGetComponent(out LockedGate lockedGate)) lockedGate.WeaponHit(collider);
        Collect();
    }

    protected override void Collect()
    {
        if (state == State.Collect) return;
        base.Collect();

        if (upgradeType == UpgradeType.Cash)
            Instantiate(GameData.Default.cashGateParticles, thisTransform.position, Quaternion.identity);

        if (secondGate) secondGate.gameObject.SetActive(false);
        if (!icon) PlayerController.Instance.Upgrade(upgradeType, fill);
        else if (fill >= maxValue) PlayerController.Instance.Upgrade(upgradeType, addValue);

        valueScaleTween.Kill();
        Destroy();
    }

    public void Disable() => DisableCollision();
    public void Enable() => EnableCollision();
}
