using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Weapon Upgrade")]
public class WeaponUpgrade : ScriptableObject
{
    public UpgradeType type;
    public float[] value;

    public float this[int lvl]
    {
        get
        {
            var addedValue = type switch
            {
                UpgradeType.Increase => GameData.IncreaseStartIndex,
                UpgradeType.Multiplier => GameData.MultiplierStartIndex,
                _ => 0
            };
            return value[Mathf.Min(lvl + addedValue, MaxLvl - 1)];
        }
    }
    public int MaxLvl => value.Length;
}