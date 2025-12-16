using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridArea : MonoBehaviour
{
    [Header("Params")] 
    [SerializeField] private int price;

    private Button button;

    public void Init(Vector2Int targetGridSize, UnlockGridArea gridArea)
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            GameData.Default.PayCash(price);
            GameData.SecondGridSize = targetGridSize;
            gridArea.RespawnArea();
        });
    }
}
