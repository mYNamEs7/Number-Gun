using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LockedGate : BulletTarget, IWeaponTarget
{
    private enum LockType
    {
        Wood, Brick
    }
    
    [SerializeField] private bool isLocked;
    
    [Header("Property")]
    [SerializeField] private LockType type;
    [SerializeField] private float maxValue;
    
    [Header("Visual")]
    [SerializeField] private GameObject visual;
    [SerializeField] private Transform brickWall;
    [SerializeField] private Transform woodWall;
    [SerializeField] private Rigidbody[] woods;
    [SerializeField] private Image progressImage;
    [SerializeField] private Gate gate;
    
    private float fill;

    private void OnValidate()
    {
        switch (isLocked)
        {
            case true when !visual.activeInHierarchy:
                visual.SetActive(true);
                gate.Disable();
                break;
            case false when visual.activeInHierarchy:
                visual.SetActive(false);
                gate.Enable();
                break;
        }

        switch (type)
        {
            case LockType.Wood:
                brickWall.gameObject.SetActive(false);
                woodWall.gameObject.SetActive(true);
                woods = woodWall.GetComponentsInChildren<Rigidbody>();
                break;
            case LockType.Brick:
                brickWall.gameObject.SetActive(true);
                woodWall.gameObject.SetActive(false);
                woods = brickWall.GetComponentsInChildren<Rigidbody>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void WeaponHit(Collider collider)
    {
        Collect();
    }

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        fill = Mathf.Min(fill + damage * multiplyDamage, maxValue);

        UpdateProgressValue();
        DetachWoods();
    }

    private void UpdateProgressValue()
    {
        var value = 1 - fill / maxValue;
        progressImage.fillAmount = value;
        
        if (value <= 0)
        {
            DisableCollision();
            
            Destroy(visual);
            gate.Enable();
            
            Destroy(this);
        }
    }

    private void DetachWoods()
    {
        var woodCount = fill / (maxValue / woods.Length);
        for (var i = 0; i < woodCount; i++)
        {
            var wood = woods[i];
            if (wood.isKinematic)
            {
                wood.isKinematic = false;
                wood.useGravity = true;
            }
        }
    }
}
