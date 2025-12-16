using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Grid;
using MyInputManager;
using TMPro;
using UnityEngine;

public class FirstTutorial : MonoBehaviour
{
    [SerializeField] GameObject hand;
    [SerializeField] TMP_Text tutorialText;

    private Coroutine routine;

    void OnEnable()
    {
        tutorialText.transform.localScale = Vector3.zero;
        if(GameData.LanguageEnable) GameDataOnOnLanguageUpdate();
        
        Level.OnStartUpgrade += StartUpgrade;
        Level.OnEndUpgrade += LevelOnOnEndUpgrade;
        GameManager.OnRestartEvent += GameManagerOnOnRestartEvent;
        GameData.OnLanguageUpdate += GameDataOnOnLanguageUpdate;
    }

    void OnDisable()
    {
        Level.OnStartUpgrade -= StartUpgrade;
        Level.OnEndUpgrade -= LevelOnOnEndUpgrade;
        GameManager.OnRestartEvent -= GameManagerOnOnRestartEvent;
        GameData.OnLanguageUpdate -= GameDataOnOnLanguageUpdate;
    }
    
    private void GameDataOnOnLanguageUpdate()
    {
        tutorialText.text = GameData.Language == Language.RU ? "размещайте \nблоки, чтобы \nмодернизировать \nсвое оружие" : "place \nblocks to \nupgrade \nyour gun";
    }
    
    private void GameManagerOnOnRestartEvent()
    {
        if (routine != null) StopCoroutine(routine);
        StopAllCoroutines();
    }

    void StartUpgrade()
    {
        // UIManager.Instance.inGameWindow.restartButton.gameObject.SetActive(false);
        // InputManager.LockControll();
        routine = StartCoroutine(Tutorial());
    }
    
    private void LevelOnOnEndUpgrade()
    {
        tutorialText.transform.localScale = Vector3.zero;
    }

    private IEnumerator TutorialText()
    {
        yield return new WaitForSeconds(2.5f);

        tutorialText.transform.DOScale(0.5f, 0.33f);
    }

    IEnumerator Tutorial()
    {
        StartCoroutine(TutorialText());
        
        yield return null;
        Card card = Level.Instance.upgradeZone.cards[0];
        Card secondCard = Level.Instance.upgradeZone.cards[1];
        
        card.DisableUsage();
        secondCard.DisableUsage();
        
        yield return new WaitWhile(() => !card.canDrag);
        //
        GameObject hand = Instantiate(this.hand, card.thisTransform.position + Vector3.up, Quaternion.identity, Level.Instance.thisTransform);
        Transform handTransform = hand.transform;
        handTransform.DOMove(handTransform.position + Vector3.back * 6 + Vector3.right * 2, 0.6f).From();
        handTransform.DORotate(new Vector3(0, 30, 0), 0f);
        yield return new WaitForSeconds(0.75f);
        handTransform.localScale *= 0.8f;
        //
        // GameObject upgradeAim = Instantiate(GameData.Default.cardUpgradeAim);
        // Transform upgradeAimTransform = upgradeAim.transform;
        // Vector3 startPoint = card.thisTransform.position;
        // card.StartDrag();
        // float t = 0;
        // while (t < 1)
        // {
        //     t += Time.deltaTime;
        //     Vector3 point = Vector3.Slerp(startPoint, PlayerController.Instance.thisTransform.position + Vector3.back * 3, t);
        //     card.thisTransform.position = Vector3.Lerp(card.thisTransform.position, point + Vector3.up * 2, Time.deltaTime * 12);
        //     handTransform.position = card.thisTransform.position + Vector3.up;
        //     upgradeAimTransform.position = card.thisTransform.position + Vector3.forward * PlayerController.UpgradeAimOffset + Vector3.up * (0.1f - card.thisTransform.position.y);
        //     card.Drag();
        //     if (t > 0.6f) PlayerController.Instance.Weapons[0].SetUpgradeTarget();
        //     yield return null;
        // }
        //
        // handTransform.localScale *= 1.25f;
        // Instantiate(GameData.Default.cardLvlUpParticles, PlayerController.Instance.thisTransform);
        // Level.Instance.upgradeZone.RemoveCard(card);
        // List<WeaponUpgrade> upgrades = card.GetUpgrades();
        // upgrades.ForEach(upgrade => PlayerController.Instance.Upgrade(upgrade.type, upgrade[card.CurLvl], true));
        // PlayerController.Instance.Weapons[0].ReleaseUpgradeTarget();
        // Destroy(upgradeAim);
        //
        // yield return new WaitForSeconds(0.33f);
        // EndTutorial();

        var grid = FindObjectOfType<UpgradeGrid>();
        yield return null;
        
        var pos = card.thisTransform.position;
        pos.y = 1f;
        handTransform.position = pos;
        
        UpgradeGrid.CanPainting = true;
        card.StartDrag();

        yield return null;
        card.EnableUsage();
        secondCard.CanDrag = false;
        UpgradeGrid.CanPainting = false;
        
        // grid.CanPainting = false;
        Tween handTween = null;
        var targetPos = grid.Cells.First(x => x.isPainted).thisTransform.position;
        targetPos.y = 1f;
        handTween = handTransform.DOMove(targetPos, 1.5f).SetLoops(-1, LoopType.Restart);
        // handTween = handTransform.GetChild(0).DOLocalMove(handTransform.GetChild(0).localPosition * 2, 0.33f).SetLoops(-1, LoopType.Yoyo);

        yield return new WaitUntil(() => card.IsUsed);
        // grid.CanPainting = true;
        card.DisableUsage();
        handTween.Kill();
        
        yield return new WaitForSeconds(0.1f);
        print("ВТОРАЯ СТАДИЯ");
        
        secondCard.CanDrag = true;

        secondCard.StartDrag();
        secondCard.EnableUsage();
        UpgradeGrid.CanPainting = false;
        
        pos = secondCard.thisTransform.position;
        pos.y = 1f;
        handTransform.position = pos;
        
        targetPos = grid.Cells.First(x => x.isPainted).thisTransform.position;
        targetPos.y = 1f;
        handTween = handTransform.DOMove(targetPos, 1.5f).SetLoops(-1, LoopType.Restart);
        
        yield return new WaitUntil(() => secondCard.IsUsed);
        secondCard.DisableUsage();

        // while (Level.Instance.Stage == LevelStage.Upgrade && card)
        // {
        //     while (!PlayerController.Instance.curDragCard)
        //     {
        //         handTransform.position = Vector3.Lerp(handTransform.position, secondCard.thisTransform.position + new Vector3(1, 0, 2), Time.deltaTime * 6);
        //         yield return null;
        //     }
        //     while (PlayerController.Instance.curDragCard)
        //     {
        //         handTransform.position = Vector3.Lerp(handTransform.position, PlayerController.Instance.thisTransform.position + new Vector3(1, 0, 1), Time.deltaTime * 6);
        //         yield return null;
        //     }
        // }

        handTween.Kill();
        handTransform.DOMove(handTransform.position + Vector3.back * 15, 0.5f).OnComplete(() => Destroy(hand));
    }

    void EndTutorial()
    {
        // UIManager.Instance.inGameWindow.restartButton.gameObject.SetActive(true);
        // InputManager.UnlockControll();
    }
}