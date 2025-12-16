using System;
using System.Collections.Generic;
using UI.Upgrade_Window;
using UnityEngine;

namespace UI.Windows
{
    public class UpgradeWindow : MonoBehaviour
    {
        [SerializeField] private FireValueUI mainDPS;

        private void OnEnable()
        {
            Level.OnStartUpgrade += LevelOnOnStartUpgrade;
            Level.OnEndUpgrade += LevelOnOnEndUpgrade;
            GameManager.OnRestartEvent += LevelOnOnEndUpgrade;

            LevelOnOnEndUpgrade();
        }

        private void OnDisable()
        {
            Level.OnStartUpgrade += LevelOnOnStartUpgrade;
            Level.OnEndUpgrade += LevelOnOnEndUpgrade;
            GameManager.OnRestartEvent -= LevelOnOnEndUpgrade;
        }

        private void LevelOnOnEndUpgrade()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            mainDPS.gameObject.SetActive(true);
        }

        private void LevelOnOnStartUpgrade()
        {
            transform.GetChild(0).gameObject.SetActive(true);
            mainDPS.gameObject.SetActive(false);
        }
    }
}
