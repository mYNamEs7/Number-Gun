using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YearsTag : MonoBehaviour
{
    public static event Action<int> OnYearsUpdate;
    
    readonly static Dictionary<Language, string> YearsName = new Dictionary<Language, string> { { Language.RU, "<size=100%>Год</size>\n<size=170%>{0}</size>" }, { Language.EN, "<size=100%>Year</size>\n<size=170%>{0}</size>" }, { Language.TR, "{0}\nYIL" } };
    [SerializeField] TMP_Text txt;
    public int year;

    void OnEnable() => GameData.OnLanguageUpdate += UpdateYear;
    void OnDisable() => GameData.OnLanguageUpdate -= UpdateYear;

    public void Set(int year)
    {
        this.year = year;
        UpdateYear();
        
        OnYearsUpdate?.Invoke(year);
    }

    void UpdateYear() => txt.SetText(string.Format(YearsName[GameData.Language], year));
}
