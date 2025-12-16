using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grid;
using MyInputManager;
using UnityEngine;

namespace UI.Windows
{
    public class BaseUpgradeWindow : MonoBehaviour
    {
        public static event Action OnStartUpgrade;
        public static event Action OnEndUpgrade;

        [NonSerialized] public Transform thisTransform;

        [SerializeField] private Transform playerPoint;
        [SerializeField] private Transform cardParent;

        private Camera cam;
        private Card curDragCard;

        private void Awake()
        {
            thisTransform = transform;
            cam = Camera.main;
        }

        private void OnEnable()
        {
            foreach (Transform card in cardParent)
            {
                if (card.TryGetComponent(out Card cardComponent))
                {
                    cardComponent.EnableCollect();
                }
            }

            StartCoroutine(StartUpgrade());
        }

        private void OnDisable()
        {
            OnEndUpgrade?.Invoke();
            InputManager.DownEvent -= TryDrag;
            InputManager.UpEvent -= EndDrag;
        }

        private IEnumerator StartUpgrade()
        {
            yield return null;

            OnStartUpgrade?.Invoke();
            InputManager.DownEvent += TryDrag;
            InputManager.UpEvent += EndDrag;

            // Level.Instance.Stage = LevelStage.Upgrade;
        }

        private void Update()
        {
            if (curDragCard)
            {
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                {
                    curDragCard.thisTransform.position = Vector3.Lerp(curDragCard.thisTransform.position, hit.point + Vector3.forward * 2, Time.deltaTime * 20);
                }
                curDragCard.Drag();
            }
        }

        private void TryDrag()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) &&
                hit.collider.TryGetComponent(out Card card))
            {
                print("DRAGG");
                if (!card.CanDrag)
                {
                    UpgradeGrid.CanPainting = false;
                }

                curDragCard = card.StartDrag();
                if (curDragCard && curDragCard.IsUsed)
                {
                    curDragCard.IsUsed = false;
                    curDragCard.RemovePoints();
                }
            }
        }

        private void EndDrag()
        {
            if (curDragCard)
            {
                Card card = curDragCard;
                curDragCard = null;

                if (card.EndDrag())
                {
                    // targetCells.ForEach(x => x.ReleaseUpgradeTarget());
                    int lvl = card.CurLvl;
                
                    card.AddPoints();
                }
            }
        }
    }
}
