using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HandPopup : PopupUI
{
    [Header("Special Refs")] 
    [SerializeField] private Image newHandImg;
    [SerializeField] private TMP_Text startIncomeTxt, startFireRateTxt, startFireRangeTxt;
    [SerializeField] private TMP_Text endIncomeTxt, endFireRateTxt, endFireRangeTxt;

    [NonSerialized] public int newHandId = -1;
    
    public override void Init()
    {
        base.Init();
        if (newHandId == -1)
            newHandId = Random.Range(0, GameData.Default.hands.Length);
        newHandImg.sprite = GameData.Default.handsUI[newHandId];
        
        var curHandData = GameData.Default.hands[GameData.CurHandId].data;
        var newHandData = GameData.Default.hands[newHandId].data;
        
        startIncomeTxt.text = $"+{curHandData.income}";
        endIncomeTxt.text = $"+{newHandData.income}";
        startFireRateTxt.text = $"+{curHandData.fireRate}";
        endFireRateTxt.text = $"+{newHandData.fireRate}";
        startFireRangeTxt.text = $"+{curHandData.fireRange}";
        endFireRangeTxt.text = $"+{newHandData.fireRange}";
    }

    protected override void OnReward()
    {
        GameData.CurHandId = newHandId;
        PlayerController.Instance.Weapons.ForEach(x => x.RespawnHand(false));
    }
}
