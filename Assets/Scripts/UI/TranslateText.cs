using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranslateText : MonoBehaviour
{
    [SerializeField] string text, ruText, trText;

    void OnEnable() => GameData.OnLanguageUpdate += Translate;
    void OnDisable() => GameData.OnLanguageUpdate -= Translate;
    void Start()
    {
        if (GameData.LanguageEnable) Translate();
    }

    void Translate()
    {
        string text = GameData.Language == Language.RU ? ruText : (GameData.Language == Language.TR ? trText : this.text);
        if (TryGetComponent(out TMP_Text TMPtxt))
        {
            TMPtxt.SetText(text);
            TMPtxt.ForceMeshUpdate(true);
        }
        else if (TryGetComponent(out Text txt)) txt.text = text;
    }
}
