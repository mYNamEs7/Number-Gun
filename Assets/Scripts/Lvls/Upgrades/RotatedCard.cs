using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatedCard : MonoBehaviour
{
    public Transform thisTransform;

    private void Awake()
    {
        thisTransform = transform;
    }

    private void OnEnable()
    {
        StartCoroutine(Rotation());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Rotation()
    {
        while (true)
        {
            thisTransform.Rotate(0, 0, 150f * Time.deltaTime);
            yield return null;
        }
    }
}
