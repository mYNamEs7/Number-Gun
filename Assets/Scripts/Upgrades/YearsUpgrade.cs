using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Years Upgrade")]
public class YearsUpgrade : Upgrade
{
    protected override float value => (CurLvl + 2) * 5 + GameData.AddedYears;
    public override float CurValue => CurLvl < 0 ? zeroUpgradesValue + GameData.AddedYears : value;
}