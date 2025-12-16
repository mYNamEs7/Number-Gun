using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DiagonalMover : MonoBehaviour
{
    [NonSerialized] public Transform thisTransform;
    
    [Header("Params")]
    [SerializeField] private Vector3 distance;
    [SerializeField] private float duration;
    
    private bool isMoved;

    private void Awake()
    {
        thisTransform = transform;
        GetComponent<Card>().OnHit += OnHit;
    }

    private void OnHit()
    {
        if (!isMoved)
        {
            thisTransform.DOMove(thisTransform.TransformPoint(distance), duration).SetEase(Ease.InFlash);
            isMoved = true;
        }
    }
}
