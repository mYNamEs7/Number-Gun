using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GunPanel : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private int years;
    [SerializeField] private int price;
    [SerializeField] private Button claimBtn;
    [SerializeField] private TMP_Text claimBtnTxt;
    
    private void Awake()
    {
        claimBtn.onClick.AddListener(() =>
        {
            if (IsAdAvailable())
                GameManager.ShowRewardVideo(OnReward);
        });
        if (GameData.GunItems.Length == 0)
            GameData.GunItems = new[] { -1, -1, -1, -1, -1, -1 };
        UpdateButton();
    }

    private bool IsAdAvailable() => id > GameData.GunItems.Length - 1 || GameData.GunItems[id] != 1;

    private void OnReward()
    {
        if (id <= GameData.GunItems.Length - 1)
            GameData.GunItems[id] = 1;
        else
            GameData.GunItems = GameData.GunItems.Append(1).ToArray();

        var value = (int)GameData.Default.GetUpgrade(UpgradeType.Years).CurValue - GameData.AddedYears;
        GameData.AddedYears = Mathf.Max(years - 1800 - value, GameData.AddedYears);
        UpdateButton();
    }

    private void UpdateButton()
    {
        if (IsAdAvailable())
        {
            claimBtn.interactable = true;
            claimBtnTxt.text = $"${price}";
        }
        else
        {
            claimBtn.interactable = false;
            var text = GameData.Language == Language.EN ? "Purchased" : "Куплено";
            claimBtnTxt.text = $"{text}";
        }
    }
}
