using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlockUpgrade : UIUpgrade
{
    [SerializeField] private Image blockImg;
    
    private Button button;
    [NonSerialized] public int blockIndex;
    
    protected override void Awake()
    {
        canShowAd = false;
        base.Awake();
    }

    protected override void Init()
    {
        base.Init();
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            BlockManager.Instance.SpawnBlock(blockIndex);
            Buy();
        });
    }

    protected override void UpdatePrice()
    {
        base.UpdatePrice();
        priceTxt.text = $"${priceTxt.text}";
    }

    protected override void UpdateLvl()
    {
        var lvl = Data.CurLvl + 1;
        var maxValues = BlockUpgrade.maxValues;

        for (int i = 0; i < (int)Data.CurValue; i++)
        {
            lvl -= maxValues[i];
        }
        lvlTxt.text = $"{lvl}/{maxValues[(int)Data.CurValue]}";
        UpdateImage();
    }

    private void UpdateImage()
    {
        blockIndex = (int)Data.CurValue;
        blockImg.sprite = GameData.Default.blockIcons[blockIndex];
    }
}
