using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIYearsSlider : MonoBehaviour
{
    readonly static Dictionary<Language, string> MaxName = new Dictionary<Language, string> { { Language.RU, "МАКС" }, { Language.EN, "MAX" }, { Language.TR, "MAKS." } };

    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI firstYears, secondYears;
    [SerializeField] TextMeshProUGUI firstName, secondName;
    [SerializeField] Image firstImage, secondImage;

    void OnEnable()
    {
        if (GameManager.GameEnabled) Init();
        else GameManager.OnStartMenu += Init;
    }

    void Init()
    {
        // GameData.Default.GetUpgrade(UpgradeType.Years).OnUpgrade += UpdateYears;
        YearsTag.OnYearsUpdate += YearsTagOnOnYearsUpdate;
        Level.OnStartUpgrade += LevelOnOnStartUpgrade;
        Level.OnEndUpgrade += LevelOnOnEndUpgrade;
        UpdateYears();
    }

    void OnDisable()
    {
        GameManager.OnStartMenu -= Init;
        // GameData.Default.GetUpgrade(UpgradeType.Years).OnUpgrade -= UpdateYears;
        YearsTag.OnYearsUpdate -= YearsTagOnOnYearsUpdate;
    }
    
    private void LevelOnOnEndUpgrade()
    {
        gameObject.SetActive(true);
        
        Level.OnStartUpgrade -= LevelOnOnStartUpgrade;
        Level.OnEndUpgrade -= LevelOnOnEndUpgrade;
        GameManager.OnRestartEvent -= LevelOnOnEndUpgrade;
    }

    private void LevelOnOnStartUpgrade()
    {
        GameManager.OnRestartEvent += LevelOnOnEndUpgrade;
        gameObject.SetActive(false);
    }
    
    private void YearsTagOnOnYearsUpdate(int years)
    {
        int max = (GameData.Default.weaponsUI.Length - 1) * 50 - 1;
        years = Mathf.Min(years - 1800, max);
        slider.value = years % 50 / 50f;
        int curLvl = years / 50;

        firstYears.text = (1800 + curLvl * 50).ToString();
        secondYears.text = years == max ? MaxName[GameData.Language] : (1850 + curLvl * 50).ToString();
        if(firstName) firstName.text = GameData.Language == Language.RU ? GameData.Default.weaponsUI[curLvl].nameRU : (GameData.Language == Language.TR ? GameData.Default.weaponsUI[curLvl].nameTR : GameData.Default.weaponsUI[curLvl].name);
        if(secondName) secondName.text = GameData.Language == Language.RU ? GameData.Default.weaponsUI[curLvl + 1].nameRU : (GameData.Language == Language.TR ? GameData.Default.weaponsUI[curLvl + 1].nameTR : GameData.Default.weaponsUI[curLvl + 1].name);
        firstImage.sprite = GameData.Default.weaponsUI[curLvl].icon;
        secondImage.sprite = GameData.Default.weaponsUI[curLvl + 1].inactiveIcon;
    }

    void UpdateYears()
    {
        print("UPDATE TEXT");
        
        int max = (GameData.Default.weaponsUI.Length - 1) * 50 - 1;
        int years = Mathf.Min((int)GameData.Default.GetUpgrade(UpgradeType.Years).CurValue, max);
        slider.value = years % 50 / 50f;
        int curLvl = years / 50;

        firstYears.text = (1800 + curLvl * 50).ToString();
        secondYears.text = years == max ? MaxName[GameData.Language] : (1850 + curLvl * 50).ToString();
        if(firstName) firstName.text = GameData.Language == Language.RU ? GameData.Default.weaponsUI[curLvl].nameRU : (GameData.Language == Language.TR ? GameData.Default.weaponsUI[curLvl].nameTR : GameData.Default.weaponsUI[curLvl].name);
        if(secondName) secondName.text = GameData.Language == Language.RU ? GameData.Default.weaponsUI[curLvl + 1].nameRU : (GameData.Language == Language.TR ? GameData.Default.weaponsUI[curLvl + 1].nameTR : GameData.Default.weaponsUI[curLvl + 1].name);
        firstImage.sprite = GameData.Default.weaponsUI[curLvl].icon;
        secondImage.sprite = GameData.Default.weaponsUI[curLvl + 1].inactiveIcon;
    }
}
