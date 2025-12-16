using TMPro;
using UnityEngine;

public class UILvlInputField : MonoBehaviour
{
    void Awake() => GetComponent<TMP_InputField>().onEndEdit.AddListener(LoadLvl);
    void LoadLvl(string lvl) => GameManager.Instance.LoadLvl(int.Parse(lvl) - 1);
}
