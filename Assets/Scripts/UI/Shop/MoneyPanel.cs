using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPanel : MonoBehaviour
{
    private const float cooldownTime = 86400f;
    
    [SerializeField] private int id;
    [SerializeField] private int moneyQuantity;
    [SerializeField] private Button claimBtn;
    [SerializeField] private TMP_Text claimBtnTxt;
    
    private DateTime nextAvailableTime;

    private void Awake()
    {
        claimBtn.onClick.AddListener(() =>
        {
            if (IsAdAvailable())
                GameManager.ShowRewardVideo(OnReward);
        });
    }

    private void OnEnable()
    {
        if (GameData.MoneyTimers.Length == 0)
            GameData.MoneyTimers = new[] { "", "", "" };
        nextAvailableTime = DateTime.TryParse(GameData.MoneyTimers[id], out nextAvailableTime) ? nextAvailableTime.AddSeconds(cooldownTime) : DateTime.Now;
        StartCoroutine(UpdateButtonStateRoutine());
    }

    private IEnumerator UpdateButtonStateRoutine()
    {
        while (enabled)
        {
            UpdateButtonState();
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnReward()
    {
        SaveBuyTime();
        UpdateButtonState();
        
        GameData.Default.AddCash(moneyQuantity);
    }

    private void SaveBuyTime()
    {
        if (id <= GameData.MoneyTimers.Length - 1)
            GameData.MoneyTimers[id] = DateTime.Now.ToString(CultureInfo.CurrentCulture);
        else
            GameData.MoneyTimers = GameData.MoneyTimers.Append(DateTime.Now.ToString(CultureInfo.CurrentCulture)).ToArray();
        nextAvailableTime = DateTime.Now.AddSeconds(cooldownTime);
    }
    
    private bool IsAdAvailable() => DateTime.Now >= nextAvailableTime;
    
    private void UpdateButtonState()
    {
        if (IsAdAvailable())
        {
            claimBtn.interactable = true;
            claimBtnTxt.text = GameData.Language == Language.EN ? "Claim" : "Забрать";
        }
        else
        {
            claimBtn.interactable = false;
            var timeLeft = nextAvailableTime - DateTime.Now;
            claimBtnTxt.text = $"{timeLeft.Hours:D2}:{timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";
        }
    }
}
