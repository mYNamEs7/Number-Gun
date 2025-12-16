using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Wall : MonoBehaviour, IWeaponTarget
{
    [NonSerialized] public Transform thisTransform;
    
    [SerializeField] int damage;
    [SerializeField] float pushStrength;
    bool invisable;

    private void Awake()
    {
        thisTransform = transform;
    }

    public void WeaponHit(Collider collider)
    {
        if (invisable) return;
        invisable = true;
        var playerPos = PlayerController.Instance.thisTransform.position.x;
        
        if (thisTransform.position.x < playerPos)
            PlayerController.Instance.PushRight(pushStrength);
        else
            PlayerController.Instance.PushLeft(pushStrength);
        
        if (thisTransform.position.x < playerPos)
            PlayerController.Instance.HorizontalClamp.x = thisTransform.position.x + thisTransform.lossyScale.x;
        else
            PlayerController.Instance.HorizontalClamp.y = thisTransform.position.x - thisTransform.lossyScale.x;
    }

    private void Update()
    {
        if (PlayerController.Instance.thisTransform.position.z > thisTransform.position.z + thisTransform.lossyScale.z / 2)
            PlayerController.Instance.HorizontalClamp = PlayerController.Instance.data.horizontalClamp;
    }
}
