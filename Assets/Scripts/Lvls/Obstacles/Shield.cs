using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : HPObstacle
{
    [SerializeField] Animator animator;
    
    protected override void Broke()
    {
        base.Broke();
        animator.SetBool("Broken", true);
    }
}
