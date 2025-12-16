using System.Collections;
using System.Collections.Generic;
using Dreamteck;
using TMPro;
using UnityEngine;

public class GateUpgradePopup : PopupUI
{
    [Header("Params")]
    [SerializeField] private int startDisplayedValue;
    [SerializeField] private int addedValue;

    [Header("Special Refs")] 
    [SerializeField] private TMP_Text startValueTxt;
    [SerializeField] private TMP_Text endValueTxt;
    [SerializeField] private TMP_Text addedValueTxt;
    
    public override void Init()
    {
        base.Init();
        startValueTxt.text = $"+{startDisplayedValue + GameData.YearGateCapacity}";
        endValueTxt.text = $"+{startDisplayedValue + GameData.YearGateCapacity + addedValue}";
        addedValueTxt.text = $"+{addedValue} years";
    }

    protected override void OnReward()
    {
        GameData.YearGateCapacity += addedValue;
        FindObjectsOfType<Gate>().ForEach(x => x.Init());
    }
}
