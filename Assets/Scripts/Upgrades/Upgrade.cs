using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

[CreateAssetMenu(menuName = "Upgrades/Upgrade")]
public class Upgrade : ScriptableObject
{
    readonly static Dictionary<UpgradeType, int> UpgradeKey = new Dictionary<UpgradeType, int> { { UpgradeType.FireRate, 0 }, { UpgradeType.Years, 1 }, { UpgradeType.Income, 2 }, { UpgradeType.FireRange, 3 }, { UpgradeType.Block, 4} };
    public UpgradeType type;

    [Serializable]
    public class Lvl
    {
        public float value;
        public int price;
    }
    public Lvl[] lvl;
    public int zeroUpgradesValue;

    public virtual float CurValue => CurLvl < 0 ? zeroUpgradesValue : value;
    protected virtual float value => 0;
    public virtual int CurPrice => 50 + (CurLvl + 1) / 4 * 50;
    // public float CurValue => CurLvl < 0 ? zeroUpgradesValue : lvl[CurLvl].value;
    // public int CurPrice => lvl[CurLvl + 1].price;

    public int CurLvl { get => YandexGame.savesData.UpgradesLvl[UpgradeKey[type]]; set { YandexGame.savesData.UpgradesLvl[UpgradeKey[type]] = value; } }
    // public int MaxLvl => lvl.Length;
    public int MaxLvl => int.MaxValue;

    public event Action OnUpgrade;
    public void LvlUp()
    {
        if (CurLvl >= MaxLvl) return;
        YandexGame.savesData.UpgradesLvl[UpgradeKey[type]]++;
        YandexGame.SaveProgress();
        OnUpgrade?.Invoke();
    }

#if UNITY_EDITOR
    public string valueFormula;
    public string priceFormula;
#endif
}

public enum UpgradeType
{
    FireRate, FireRange, Years, Months, Income, Cash, GunAmount, GunAmountMultiply, MultiShot, BulletSize, Damage, Multiplier, Increase, Block
}