using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoldenGunPopup : PopupUI
{
    [Header("Special Refs")]
    [SerializeField] private Image curWeaponImg;
    [SerializeField] private Image goldenWeaponImg;
    [SerializeField] private TMP_Text startDamageTxt, startFireRateTxt, startFireRangeTxt;
    [SerializeField] private TMP_Text endDamageTxt, endFireRateTxt, endFireRangeTxt;

    public override void Init()
    {
        base.Init();

        var weapon = PlayerController.Instance.Weapons[0];
        var curLvl = weapon.data.lvl;
        curWeaponImg.sprite = GameData.Default.weaponsUI[curLvl].icon;
        goldenWeaponImg.sprite = GameData.Default.weaponsUI[curLvl].goldenIcon;
        
        startDamageTxt.text = $"+{weapon.data.damage}";
        endDamageTxt.text = $"+{weapon.data.damage * 2}";
        startFireRateTxt.text = $"+{GameData.GoldenGunFireRate}";
        endFireRateTxt.text = $"+{GameData.GoldenGunFireRate + 3}";
        startFireRangeTxt.text = $"+{GameData.GoldenGunFireRange}";
        endFireRangeTxt.text = $"+{GameData.GoldenGunFireRange + 3}";
    }

    protected override void OnReward()
    {
        PlayerController.Instance.Weapons.ForEach(x => x.SetGolden(x.data.lvl));
    }
}
