using UnityEngine;

[CreateAssetMenu(menuName = "Data/Chest Data")]
public class ChestData : ScriptableObject
{
    [System.Serializable]
    public class CardPack
    {
        public ChestCard[] cards;
    }
    public CardPack[] packs;
}