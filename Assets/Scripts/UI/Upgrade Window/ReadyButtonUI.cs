using System;
using Grid;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Upgrade_Window
{
    public class ReadyButtonUI : MonoBehaviour
    {
        [SerializeField] private GameObject inactiveButton;
        [SerializeField] private GameObject activeButton;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnEnable()
        {
            UpgradeGrid.OnCardCountChanged += UpgradeGridOnOnCardCountChanged;
            
            UpgradeGridOnOnCardCountChanged(0);
        }

        private void OnDisable()
        {
            UpgradeGrid.OnCardCountChanged -= UpgradeGridOnOnCardCountChanged;
        }

        private void UpgradeGridOnOnCardCountChanged(int amount)
        {
            var isActive = amount > 0;
            if (Level.Instance.TryGetComponent(out FirstTutorial _))
                isActive = amount >= 2;

            button.enabled = isActive;

            if (PlayerController.Instance.Weapons != null)
            {
                var weapon = PlayerController.Instance.Weapons[0];
                if (isActive) weapon.TransformRightGunPointZ();
                else weapon.TransformLeftGunPointZ();
            }
            
            activeButton.SetActive(isActive);
            inactiveButton.SetActive(!isActive);
        }

        public void OnClick() => Level.Instance.Stage = LevelStage.Second;
    }
}
