using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/FireRate Upgrade")]
public class FireRateUpgrade : Upgrade
{
    protected override float value
    {
        get
        {
            var value = GameData.Default.fireRate;
            for (int i = 0; i < CurLvl + 1; i++)
            {
                value += value * 0.05f;
            }

            return value;
        }
    }
    // protected override float value => CurValue + CurValue * 0.02f;
}