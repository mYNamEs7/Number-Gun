using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Vector3Int axis = Vector3Int.right;
    [SerializeField] AnimationCurve animationCurve;
    Transform thisTransform;

    void Start()
    {
        thisTransform = transform;
    }

    void Update()
    {
        thisTransform.localRotation = Quaternion.Euler((Vector3)axis * (360f * animationCurve.Evaluate(Time.time * speed % 1)));
    }
}
