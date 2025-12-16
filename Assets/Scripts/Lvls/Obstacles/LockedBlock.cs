using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LockedBlock : BulletTarget, IWeaponTarget
{
    [SerializeField] private Transform block;
    
    [Header("Params")]
    [SerializeField] private float maxValue;

    [Header("Refs")] 
    [SerializeField] private GameObject progressSlider;
    [SerializeField] private Image progressImage;
    [SerializeField] private Animator animator;
    
    private float fill;
    private Vector3 statBlockScale;
    private static readonly int Open = Animator.StringToHash("Open");

    protected override void Awake()
    {
        base.Awake();
        statBlockScale = block.localScale;
        block.localScale = Vector3.zero;
    }

    public void WeaponHit(Collider collider)
    {
        Collect();
    }

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        fill = Mathf.Min(fill + damage * multiplyDamage, maxValue);

        UpdateProgressValue();
    }

    private void UpdateProgressValue()
    {
        var value = 1 - fill / maxValue;
        progressImage.fillAmount = value;
        
        if (value <= 0)
        {
            DisableCollision();
            
            progressSlider.SetActive(false);
            animator.SetTrigger(Open);
            block.DOScale(statBlockScale, 0.33f).SetEase(Ease.OutElastic);
            
            Destroy(this);
        }
    }
}
