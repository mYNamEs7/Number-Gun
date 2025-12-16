using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimatedElementUI : MonoBehaviour
{
    [NonSerialized] public Transform thisTransform;

    [SerializeField] private float showDelay;
    [SerializeField] private float hideDelay;

    private void Awake()
    {
        thisTransform = transform;
        thisTransform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        thisTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetDelay(showDelay);
    }
}
