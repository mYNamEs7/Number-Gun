using System;
using Grid;
using TMPro;
using UnityEngine;

namespace UI.Upgrade_Window
{
    public class FireValueUI : MonoBehaviour
    {
        [SerializeField] private bool isBasedMultiplier;
        [SerializeField] private TMP_Text text;
        [SerializeField] private string additionalText_en;
        [SerializeField] private string additionalText_ru;
        [SerializeField] private bool addTextInFront;
        
        private void OnEnable()
        {
            PlayerController.Instance.OnFireRateChanged += UpgradeGridOnOnFireValueChanged;
            // UpgradeGridOnOnFireValueChanged(PlayerController.Instance.FireRate);
        }

        private void UpgradeGridOnOnFireValueChanged(float value)
        {
            if (isBasedMultiplier) value = GameData.BaseMultiplier;
            else if (PlayerController.Instance && PlayerController.Instance.Weapons.Count > 0)
            {
                print(GameData.BaseMultiplier);
                value *= Weapon.fireRateByYears * PlayerController.Instance.Weapons.Count * PlayerController.Instance.Weapons[0].bulletCount * GameData.BaseMultiplier;
                value += PlayerController.Instance.BulletSize;
            }
            
            var txt = GameData.Language == Language.EN ? additionalText_en : additionalText_ru;
            text.text = $"{(float)Math.Round(value, 2)}";
            text.text = addTextInFront ? txt + text.text : text.text + txt;
        }
    }
}
