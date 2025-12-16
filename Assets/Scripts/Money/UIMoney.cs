using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIMoney : MonoBehaviour
{
    #region Singletone
    private static UIMoney _instance;
    public static UIMoney Instance { get => _instance; }
    public UIMoney() => _instance = this;
    #endregion

    [Header("Cash")]
    [SerializeField] private TextMeshProUGUI txt;
    private int oldMoney;
    private Transform txtTransform;
    private Tween moneyJump, moneyColor;

    [Header("Keys")]
    [SerializeField] GameObject keysCounter;
    [SerializeField] TextMeshProUGUI keysCountText;
    Transform keysCounterTransform;
    public Vector3 KeysCounterPos => keysCounterTransform.position;
    int oldKeysCount;
    private Tween keysColor;

    public void Init()
    {
        txtTransform = txt.transform;
        keysCounterTransform = keysCounter.transform;
        if (txt) SetCash(GameData.Cash);
        if (keysCountText) SetKeys(GameData.Keys);
    }
    
    public void SetCash(int cash)
    {
        txt.text = FormatNumber(cash);

        if (oldMoney < cash)
        {
            moneyJump.Kill(true);
            moneyColor.Kill(true);
            moneyJump = txtTransform.DOLocalMoveY(25, 0.2f).From().SetUpdate(true);
            moneyColor = txt.DOColor(new Color(0.166f, 1, 0.07f, 1), 0.2f).From().SetUpdate(true);
        }

        oldMoney = cash;
    }

    public void SetKeys(int keys)
    {
        keysCountText.text = $"{keys}/10";
        if (oldKeysCount < keys)
        {
            keysColor.Kill(true);
            keysColor = keysCountText.DOColor(new Color(1, 0.84f, 0.14f, 1), 0.2f).From().SetUpdate(true);
        }

        oldKeysCount = keys;
        keysCounter.SetActive(keys > 0);
    }

    string FormatNumber(int number)
    {
        if (number < 1000) return number.ToString();

        int k = number / 1000;
        int notK = number - k * 1000;
        int h = notK / 100;
        return k + "." + h + ((notK - h * 100) / 10) + "K";
    }
}