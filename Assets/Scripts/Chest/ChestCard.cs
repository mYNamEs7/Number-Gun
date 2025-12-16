using System;
using UnityEngine;
using UnityEngine.UI;

public class ChestCard : MonoBehaviour
{
    enum Type
    {
        AddYears, DoubleGun, DoubleIncome, AllUpgrades, AddCash
    }
    [SerializeField] Type type;
    [SerializeField] int value;
    [SerializeField] GameObject chooseMask;

    public event Action<ChestCard> OnClickEvent;

    public void Enable() => GetComponent<Button>().interactable = true;

    public void Click() => OnClickEvent?.Invoke(this);

    public void Choose() => chooseMask.SetActive(true);
    public void Unchoose() => chooseMask.SetActive(false);

    public void Get()
    {
        switch (type)
        {
            case Type.AddYears:
                {
                    GameManager.OnPostStartMenu += AddYears;
                }
                break;
            case Type.DoubleGun:
                {
                    GameManager.OnPostStartMenu += DoubleGun;
                }
                break;
            case Type.DoubleIncome:
                {
                    GameManager.OnPostStartMenu += DoubleIncome;
                }
                break;
            case Type.AllUpgrades:
                {
                    GameData.Default.upgrades.ForEach(x => x.LvlUp());
                }
                break;
            case Type.AddCash:
                {
                    UIManager.GetReward(transform.localPosition, value);
                }
                break;
        }
    }

    static void AddYears()
    {
        PlayerController.Instance.Upgrade(UpgradeType.Years, 200, false);
        GameManager.OnPostStartMenu -= AddYears;
    }

    static void DoubleGun()
    {
        PlayerController.Instance.Upgrade(UpgradeType.GunAmountMultiply, 2, false);
        GameManager.OnPostStartMenu -= DoubleGun;
    }

    static void DoubleIncome()
    {
        GameData.Default.cashRewardMultiplier = 2;
        GameManager.OnPostStartMenu -= DoubleIncome;
    }
}