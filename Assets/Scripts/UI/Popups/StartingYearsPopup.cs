using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class StartingYearsPopup : PopupUI
{
    [Header("Special Refs")] 
    [SerializeField] private Image newWeaponImg;
    [SerializeField] private TMP_Text startDamageTxt, startFireRateTxt, startFireRangeTxt;
    [SerializeField] private TMP_Text endDamageTxt, endFireRateTxt, endFireRangeTxt;
    
    public override void Init()
    {
        var weapon = PlayerController.Instance.Weapons[0];
        var curLvl = weapon.data.lvl;
        var newWeapon = GameData.Default.weapons[curLvl + 1];
        
        if (curLvl + 1 > GameData.Default.weapons.Length - 1)
        {
            Destroy();
            return;
        }
        
        base.Init();
        newWeaponImg.sprite = GameData.Default.weaponsUI[curLvl + 1].icon;
        
        startDamageTxt.text = $"+{weapon.data.damage}";
        endDamageTxt.text = $"+{newWeapon.data.damage}";
        startFireRateTxt.text = $"+{GameData.GoldenGunFireRate}";
        endFireRateTxt.text = $"+{GameData.GoldenGunFireRate + 5}";
        startFireRangeTxt.text = $"+{GameData.GoldenGunFireRange}";
        endFireRangeTxt.text = $"+{GameData.GoldenGunFireRange + 5}";
    }

    protected override void OnReward()
    {
        for (var i = 0; i < 10; i++)
            GameData.Default.GetUpgrade(UpgradeType.Years).LvlUp();
        YandexGame.SaveProgress();
    }
}
