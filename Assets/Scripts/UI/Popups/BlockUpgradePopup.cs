using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BlockUpgradePopup : PopupUI
{
    [Header("Special Refs")]
    [SerializeField] private TMP_Text curBlockValueTxt;
    [SerializeField] private TMP_Text nextBlockValueTxt;
    [SerializeField] private WeaponUpgrade upgrade;
    [SerializeField] private Image blockImg;

    [NonSerialized] public Sprite blockIcon;
    
    public override void Init()
    {
        if (Mathf.Approximately(upgrade[0], upgrade[1]))
        {
            Destroy();
            return;
        }
        
        base.Init();
        if (blockImg && blockIcon)
            blockImg.sprite = blockIcon;
        var startTxt = upgrade.type == UpgradeType.Increase ? "+" : "x";
        curBlockValueTxt.text = $"{startTxt}{upgrade[0]}";
        nextBlockValueTxt.text = $"{startTxt}{upgrade[1]}";
    }

    protected override void OnReward()
    {
        switch (upgrade.type)
        {
            case UpgradeType.Increase:
                GameData.IncreaseStartIndex++;
                break;
            case UpgradeType.Multiplier:
                GameData.MultiplierStartIndex++;
                break;
        }
        FindObjectsOfType<Card>().ForEach(x => x.Translate());
    }
}
