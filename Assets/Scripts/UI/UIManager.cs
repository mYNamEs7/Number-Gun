using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    public const int blockPopupIndex = 0;
    public const int usualPopupStartIndex = 1;
    
    #region Singletone
    private static UIManager _instance;
    public static UIManager Instance { get => _instance; }
    public UIManager() => _instance = this;
    #endregion

    readonly static Dictionary<Language, string> LvlName = new Dictionary<Language, string> { { Language.RU, "УРОВЕНЬ " }, { Language.EN, "LEVEL " }, { Language.TR, "SEVİYE " } };

    [Header("Main")] 
    [SerializeField] public Transform popupCanvas;
    [NonSerialized] public Transform canvas;
    [SerializeField] TMP_Text levelTxt;

    [Header("Start Window")]
    [SerializeField] StartWindow startWindow;

    [Header("In Game")]
    public InGameWindow inGameWindow;

    [Header("End Windows")]
    [SerializeField] NewWeaponWindow newWeaponWindow;
    [SerializeField] PopupWindow[] newCardWindows;
    [SerializeField] ChestWindow chestWindow;
    [SerializeField] WinWindow winWindow;
    [SerializeField] LoseWindow loseWindow;
    WindowList endWindowList;
    [NonSerialized] public int curLvlReward;

    public static event Action OnNextLvlEvent;
    public static event Action OnRestartEvent;

    void OnEnable()
    {
        GameData.UpdateCashEvent += UpdateCash;
        GameData.UpdateKeysEvent += UpdateKeys;
        GameData.AddCashEvent += AddLvlReward;
        GameManager.OnRestartEvent += ResetLvlReward;
        GameManager.OnStartMenu += ShowStartWindow;
        GameManager.OnStartGame += StartGame;
        InGameWindow.OnRestartEvent += RestartGame;
    }

    void OnDisable()
    {
        GameData.UpdateCashEvent -= UpdateCash;
        GameData.UpdateKeysEvent -= UpdateKeys;
        GameData.AddCashEvent -= AddLvlReward;
        GameManager.OnRestartEvent -= ResetLvlReward;
        GameManager.OnStartMenu -= ShowStartWindow;
        GameManager.OnStartGame -= StartGame;
        InGameWindow.OnRestartEvent -= RestartGame;
    }

    public void Init()
    {
        canvas = transform;

        UIMoney.Instance.Init();
        UpdateCash();

        endWindowList = new(new List<Window> { loseWindow });
        endWindowList.EndShowEvent += NextLvl;

        startWindow.Show();
    }

    void ResetLvlReward() => curLvlReward = 0;
    void AddLvlReward(int cash) => curLvlReward += cash;

    void UpdateCash()
    {
    }

    void UpdateKeys()
    {
    }

    void ShowStartWindow()
    {
        startWindow.Show();
        levelTxt.text = LvlName[GameData.Language] + ((LevelManager.CompleteLevelCount < LevelManager.Default.Levels.Count ? LevelManager.Default.CurrentLevelIndex : LevelManager.CompleteLevelCount) + 1);

        if (GameManager.CompleteLevelCount > 0 && GameManager.CompleteLevelCount % 2 == 0)
            ShowPopupUI(0.2f, UnityEngine.Random.Range(usualPopupStartIndex, GameData.Default.popups.Length));
    }

    public void ShowPopupUI(float waitTime, int index) => StartCoroutine(ShowPopup(waitTime, index));

    private IEnumerator ShowPopup(float waitTime, int index)
    {
        yield return new WaitForSeconds(waitTime);
        
        var popup = Instantiate(GameData.Default.popups[index], popupCanvas);
        popup.Init();
    }

    void StartGame()
    {
        startWindow.Hide();
        inGameWindow.Show();
    }

    public void ShowEndWindow(bool win)
    {
        for (int i = 0; i < GameData.Default.newCards.Length; i++) if (GameData.Default.newCards[i].lvlReceive == LevelManager.CompleteLevelCount) endWindowList.Push(newCardWindows[i], true);
        if (GameData.Keys >= 10) endWindowList.Push(chestWindow, true);
        if (win) endWindowList.Push(newWeaponWindow, true);
        DOTween.Sequence().SetDelay(1).OnComplete(() => endWindowList.Show());
    }

    void RestartGame() => OnRestartEvent?.Invoke();
    void NextLvl() => OnNextLvlEvent?.Invoke();

    public void Pause() => GameManager.Instance.PauseGame();
    public void ResumeGame() => GameManager.Instance.ResumeGame();

    public static void GetReward(Vector2 pos, int reward)
    {
        int cashCount = Mathf.Min(reward / 5, 10);
        int rewardMoney = reward;
        GameData.Cash += reward;
        for (int i = 0; i < cashCount; i++)
        {
            int n = i;
            int money = i == (cashCount - 1) ? rewardMoney : Mathf.RoundToInt(reward / (float)cashCount);
            rewardMoney -= money;
            DOTween.Sequence().SetDelay(n * 0.05f).OnComplete(() =>
            {
                GameObject cash = Instantiate(GameData.Default.cashUI, Instance.canvas);
                Transform cashTransform = cash.transform;
                cashTransform.DOScale(0, 0.15f).From();
                cashTransform.localPosition = pos;
                cashTransform.DOLocalMove(pos + UnityEngine.Random.insideUnitCircle * 100, 0.15f).OnComplete(() =>
                cashTransform.DOLocalMove(UIMoney.Instance.transform.localPosition, 0.25f).SetDelay(0.3f).OnComplete(() =>
                {
                    Destroy(cash);
                    GameData.Default.AddUICash(money);
                    SoundHolder.Default.PlayFromSoundPack("Money");
                })
                );
            });
        }

        SoundHolder.Default.PlayFromSoundPack("Reward");
    }

    #region Animations
    static Dictionary<int, Tween> tweens;
    static UIManager()
    {
        tweens = new();
    }

    public static Tween ShowElement(Component element, float delay = 0, Action action = null) => ShowElement(element.gameObject, delay, action);
    public static Tween ShowElement(GameObject element, float delay = 0, Action action = null)
    {
        if (element.TryGetComponent(out Button button)) button.interactable = true;

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;
        element.SetActive(false);

        tween = transform.DOScale(0, 0.33f).From().SetDelay(delay).SetEase(Ease.OutBack).SetUpdate(true);
        tweens[hash] = tween;
        action = (() => tweens.Remove(hash)) + action;
        tween.OnStart(() => element.SetActive(true)).OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween HideElement(Component element, float delay = 0, Action action = null) => HideElement(element.gameObject, delay, action);
    public static Tween HideElement(GameObject element, float delay = 0, Action action = null)
    {
        if (element.TryGetComponent(out Button button)) button.interactable = false;

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;
        Vector3 scale = transform.localScale;

        tween = transform.DOScale(0, 0.33f).SetDelay(delay).SetEase(Ease.InBack).SetUpdate(true);
        tweens[hash] = tween;
        action = () => { element.SetActive(false); transform.localScale = scale; tweens.Remove(hash); } + action;
        tween.OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween SlideOutElement(Component element, Vector2 side = default, float delay = 0, Action action = null) => SlideOutElement(element.gameObject, side, delay, action);
    public static Tween SlideOutElement(GameObject element, Vector2 side = default, float delay = 0, Action action = null)
    {
        if (element.TryGetComponent(out Button button)) button.interactable = true;

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;
        element.SetActive(false);

        if (side == default) side = Vector2.right;
        if (side.x == 0) tween = transform.DOLocalMoveY(Screen.height * 2 * side.y, 0.4f).From();
        else if (side.y == 0) tween = transform.DOLocalMoveX(Screen.width * 2 * side.x, 0.4f).From();
        else tween = transform.DOLocalMove(new Vector2(Screen.width * 2 * side.x, Screen.height * 2 * side.y), 0.4f).From();
        tween.SetDelay(delay).SetEase(Ease.OutBack).SetUpdate(true);

        tweens[hash] = tween;
        action = () => { tweens.Remove(hash); } + action;
        tween.OnStart(() => element.SetActive(true)).OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween SlideInElement(Component element, Vector2 side = default, float delay = 0, Action action = null) => SlideInElement(element.gameObject, side, delay, action);
    public static Tween SlideInElement(GameObject element, Vector2 side = default, float delay = 0, Action action = null)
    {
        if (element.TryGetComponent(out Button button)) button.interactable = false;

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;
        Vector3 pos = transform.localPosition;

        if (side == default) side = Vector2.right;
        if (side.x == 0) tween = transform.DOLocalMoveY(Screen.height * 2 * side.y, 0.4f);
        else if (side.y == 0) tween = transform.DOLocalMoveX(Screen.width * 2 * side.x, 0.4f);
        else tween = transform.DOLocalMove(new Vector2(Screen.width * 2 * side.x, Screen.height * 2 * side.y), 0.4f);
        tween.SetDelay(delay).SetUpdate(true);

        tweens[hash] = tween;
        action = () => { element.SetActive(false); transform.localPosition = pos; tweens.Remove(hash); } + action;
        tween.OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween ScaleAnimation(Component element, Vector3 scale = new Vector3(), float delay = 0) => ScaleAnimation(element.gameObject, scale, delay);
    public static Tween ScaleAnimation(GameObject element, Vector3 scale = new Vector3(), float delay = 0)
    {
        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;
        if (scale == new Vector3()) scale = Vector3.one * 0.15f;
        Vector3 startScale = transform.localScale;

        tween = transform.DOPunchScale(scale, 0.66f, 0, 0).SetDelay(delay).SetUpdate(true).SetAutoKill(false).OnComplete(() => tween.Restart()).OnKill(() => { transform.localScale = startScale; tweens.Remove(hash); });
        tweens[hash] = tween;
        return tween;
    }

    public static Tween ShakeAnimation(Component element, float duration = 1.0f, float strength = 9.0f, float delay = 0) => ShakeAnimation(element.gameObject, duration, strength, delay);
    public static Tween ShakeAnimation(GameObject element, float duration = 1.0f, float strength = 9.0f, float delay = 0)
    {
        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;

        tween = transform.DOShakeRotation(duration, strength).SetDelay(delay).SetUpdate(true).SetAutoKill(false).OnComplete(() => tween.Restart()).OnKill(() => tweens.Remove(hash));
        tweens[hash] = tween;
        return tween;
    }

    public static void StopAnimation(GameObject element)
    {
        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);
    }

    public static Tween TryGetTween(GameObject element)
    {
        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) return tween;
        return null;
    }
    #endregion
}