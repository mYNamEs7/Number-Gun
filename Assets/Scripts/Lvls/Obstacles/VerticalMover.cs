using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMover : MonoBehaviour
{
    [NonSerialized] public Transform thisTransform;
    
    [Header("Params")]
    [SerializeField] private float distance;
    
    private Coroutine moveRoutine;

    private void Awake()
    {
        thisTransform = transform;
        GetComponent<Card>().OnHit += OnHit;
    }

    private void OnHit() => moveRoutine ??= StartCoroutine(Move());

    private IEnumerator Move()
    {
        var target = thisTransform.position;
        target.z += distance;
        
        while (Vector3.Distance(thisTransform.position, target) > 0.1f)
        {
            thisTransform.position = Vector3.MoveTowards(thisTransform.position, target, PlayerController.Instance.data.forwardSpeed * Time.deltaTime);
            yield return null;
        }

        thisTransform.position = target;
        Destroy(this);
    }
}
