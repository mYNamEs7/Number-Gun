using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterFirstLevel : MonoBehaviour
{
    private void OnEnable()
    {
        if (LevelManager.CompleteLevelCount == 0) gameObject.SetActive(false);
    }
}
