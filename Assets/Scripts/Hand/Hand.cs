using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [NonSerialized] public Transform thisTransform;
    
    public HandData data;

    private void Awake()
    {
        thisTransform = transform;
    }
}
