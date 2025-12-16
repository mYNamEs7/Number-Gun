using UnityEngine;
using UnityEngine.Serialization;

public class Cash : Item
{
    bool collect;
    public int reward = 5;
    
    public override void WeaponHit(Collider collider)
    {
        if (collect) return;
        collect = true;
        
        Instantiate(GameData.Default.cashParticles, thisTransform.position, Quaternion.identity);
        int reward = Level.Instance.finishZone.thisTransform.position.z > thisTransform.position.z ? this.reward : (GameData.Default.startCashReward + Mathf.RoundToInt(GameData.Default.GetUpgrade(UpgradeType.Income).CurValue + GameData.AddedIncome) + GameData.Default.cashRewardAddIncome) * GameData.Default.cashRewardMultiplier;
        GameData.Default.AddCash(reward);
        SoundHolder.Default.PlayFromSoundPack("Money");
        
        base.WeaponHit(collider);
        Destroy(gameObject);
    }
}