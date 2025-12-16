using System.Collections;
using System.Collections.Generic;
using Dreamteck;
using UnityEngine;

public class Baloon : BulletTarget
{
    [SerializeField] private Card[] detailsToFill;
    
    [Header("Params")]
    [SerializeField] private int hp = 3;

    [SerializeField] private float addScaleValue = 0.33f;
    [SerializeField, Range(0, 1)] private float fill;
    
    [Header("Refs")]
    [SerializeField] private Transform baloon;
    [SerializeField] private ParticleSystem particles;

    private int curHP;

    protected override void Awake()
    {
        base.Awake();
        curHP = hp;
    }
    
    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        baloon.localScale += Vector3.one * addScaleValue;
        
        curHP -= damage;
        if (curHP <= 0)
            Broke();
    }
    
    private void Broke()
    {
        particles.transform.parent = null;
        particles.Play();
        
        detailsToFill.ForEach(x => x.SetFill(fill));
        
        Destroy(gameObject);
        Collect();
    }
}
