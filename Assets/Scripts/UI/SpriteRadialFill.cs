using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRadialFill : MonoBehaviour
{

    [SerializeField] AnimationCurve curve;
    Material material;

    public void Init()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        material = new Material(spriteRenderer.sharedMaterial);
        spriteRenderer.sharedMaterial = material;
        material.SetFloat("_Fill", 0);
    }

    public void Fill(float t)
    {
        material.SetFloat("_Fill", curve.Evaluate(t));
    }
}
