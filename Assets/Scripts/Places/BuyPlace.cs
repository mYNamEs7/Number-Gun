using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BuyPlace : LoadPlace
{

    Action buyAction;
    string key;

    [Header("UI")]
    [SerializeField] private TextMeshPro priceTxt;
    [HideInInspector] public int price;
    private int curMoney;

    public void Init(int price, Action buyAction, string name)
    {
        this.price = price;
        this.buyAction += buyAction;

        key = name + " Buy Place Cur Money";
        curMoney = PlayerPrefs.GetInt(key);
        UpdatePrice();

        fillSprite.Init();
        fillSprite.Fill(curMoney / (float)price);
    }

    protected override void StartLoad()
    {
        coroutine = StartCoroutine(Payment());
    }

    IEnumerator Payment()
    {
        float subMoney = 1;
        while (true)
        {
            int money = Mathf.Min((int)subMoney, GameData.Cash);
            if (money == 0) yield break;

            curMoney += money;
            UpdatePrice();
            LoadUpdate(curMoney / (float)price);
            PlayerPrefs.SetInt(key, curMoney);
            GameData.Default.PayCash(money);

            if (price == curMoney)
            {
                buyAction.Invoke();
                SoundHolder.Default.PlayFromSoundPack("Buy");
                break;
            }

            subMoney = Mathf.Min(subMoney + Time.deltaTime, price - curMoney);
            yield return null;
        }
    }

    protected override void EndLoad()
    {
        if (coroutine != null) StopCoroutine(coroutine);
    }

    public void UpdatePrice()
    {
        priceTxt.text = (price - curMoney).ToString();

        if (GameData.Cash >= price - curMoney) priceTxt.color = Color.white;
        else priceTxt.color = new Color(1, 0.05f, 0.03f, 1);
    }
}
