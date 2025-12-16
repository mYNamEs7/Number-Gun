using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Income Upgrade")]
public class IncomeUpgrade : Upgrade
{
    // protected override float value => CurLvl + 1;
    protected override float value
    {
        get
        {
            var value = 1f;
            for (int i = 0; i < CurLvl + 1; i++)
            {
                value += value * 0.05f;
            }

            return value;
        }
    }
}