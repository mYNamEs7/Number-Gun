using System;
using System.IO;
using System.Threading.Tasks;
using DG.Tweening;
using Grid;
using MyInputManager;
using UnityEngine;
using YG;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Singletone
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }
    public GameManager() => _instance = this;
    #endregion

    [SerializeField] private DataHolder[] dataHolders;
    Material startSkybox;

    public static bool GameEnabled;
    public static event Action OnInitEvent;
    public static event Action OnStartMenu, OnPostStartMenu;
    public static event Action OnStartGame;
    public static event Action<bool> OnFinishEvent;
    public static event Action OnRestartEvent;

    GameState state;
    public GameState State
    {
        get => state;
        set
        {
            if (state == value) return;
            //
            switch (state)
            {
                case GameState.StartMenu:
                    {
                        InputManager.DownEvent -= StartGame;
                    }
                    break;
            }
            //
            switch (value)
            {
                case GameState.StartMenu:
                    {
                        OnStartMenu?.Invoke();
                        
                        UITransition transition = Instantiate(GameData.Default.transition, UIManager.Instance.canvas);
                        transition.Open(() =>
                        {
                            Destroy(transition.gameObject);
                        });
                        InputManager.DownEvent += StartGame;

                        OnPostStartMenu?.Invoke();
                    }
                    break;
                case GameState.Game:
                    {
                        OnStartGame?.Invoke();
                    }
                    break;
            }
            //
            state = value;
        }
    }

    public static bool CanWatchAD => LevelManager.CompleteLevelCount >= 2;
    public static int CompleteLevelCount { get; private set; }

    void Awake()
    {
        if (YandexGame.SDKEnabled) YGInit();
        else YandexGame.GetDataEvent += YGInit;
    }

    void YGInit()
    {
        Application.targetFrameRate = -1;
        for (int i = 0; i < dataHolders.Length; i++) dataHolders[i].Init();

        LevelManager.Default.Init();

        UIManager.Instance.Init();
        UIManager.OnNextLvlEvent += LoadNextLvl;
        UIManager.OnRestartEvent += ReloadLvl;

        PlayerController.Instance.Init();

        CameraController.Instance.Init();
        CameraController.MainOffset = CameraController.Instance.thisTransform.position;
        CameraController.MainAngle = CameraController.Instance.thisTransform.eulerAngles;

        SoundHolder.Default.PlayFromSoundPack("Background Music", null, true);

        State = GameState.StartMenu;

        GameEnabled = true;
        OnInitEvent?.Invoke();
    }

    void StartGame() => State = GameState.Game;

    public void Finish(bool win)
    {
        if (state == GameState.Finish) return;
        CompleteLevelCount++;
        State = GameState.Finish;

        GameData.Default.AddCash(30);

        UIManager.Instance.ShowEndWindow(win);

        LevelManager.CompleteLevelCount++;
        OnFinishEvent?.Invoke(win);
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        File.Delete(Path.Combine(Application.persistentDataPath, "GameData.json"));
        if (File.Exists(Path.Combine(Application.dataPath, @"YandexGame\WorkingData\Editor\SavesEditorYG.json"))) File.Delete(Path.Combine(Application.dataPath, @"YandexGame\WorkingData\Editor\SavesEditorYG.json"));
        YandexGame.ResetSaveProgress();
    }

    public void NoneState() => State = GameState.None;
    public void StartMenuState() => State = GameState.StartMenu;

    public void LoadPrevLvl() => LoadLvl(Mathf.Max(LevelManager.Default.CurrentLevelIndex - 1, 0));
    public void ReloadLvl() => LoadLvl(LevelManager.Default.CurrentLevelIndex);

    public void SimpleReloadLvl()
    {
        LevelManager.CurrentLevel = LevelManager.Default.CurrentLevelIndex;
        DOTween.KillAll(true);
        DOTween.KillAll(true);
        DOTween.KillAll(true);
        ResetSkybox();
        LevelManager.Default.SelectLevel(LevelManager.Default.CurrentLevelIndex);
        OnRestartEvent?.Invoke();
        State = GameState.None;
        State = GameState.StartMenu;
    }

    public void LoadNextLvl()
    {
        LoadLvl(LevelManager.Default.CurrentLevelIndex + 1);
    }
    public void LoadLvl(int lvl)
    {
        // UpgradeGrid.FireValue = GameData.Default.fireRate * GameData.Default.GetUpgrade(UpgradeType.FireRate).CurValue;
        
        LevelManager.CurrentLevel = lvl;
        UITransition transition = Instantiate(GameData.Default.transition, UIManager.Instance.canvas);
        transition.Close(() =>
        {
            DOTween.KillAll(true);
            DOTween.KillAll(true);
            DOTween.KillAll(true);
            ResetSkybox();
            LevelManager.Default.SelectLevel(lvl);
            Destroy(transition.gameObject);
            OnRestartEvent?.Invoke();
            State = GameState.None;
            State = GameState.StartMenu;
        });
        YandexGame.FullscreenShow();
    }

    float timeScaleBeforePause;
    public void PauseGame()
    {
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0;
        AudioListener.pause = true;
        
        YandexGame.GameplayStop();
        print("Gameplay Stop");
    }
    public void ResumeGame()
    {
        Time.timeScale = timeScaleBeforePause;
        AudioListener.pause = false;
        
        YandexGame.GameplayStart();
        print("Gameplay Start");
    }

    void ResetSkybox()
    {
        // RenderSettings.skybox = startSkybox;
    }

    #region AD
    void OnEnable()
    {
        YandexGame.RewardVideoEvent += GetReward;
        YandexGame.CloseFullAdEvent += CloseFullAdEvent;
        YandexGame.OpenFullAdEvent += OpenFullAdEvent;
    }

    void OnDisable()
    {
        YandexGame.RewardVideoEvent -= GetReward;
        YandexGame.CloseFullAdEvent -= CloseFullAdEvent;
        YandexGame.OpenFullAdEvent -= OpenFullAdEvent;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            YandexGame.GameplayStart();
            print("Gameplay Start");
        }
        else
        {
            YandexGame.GameplayStop();
            print("Gameplay Stop");
        }
    }
    
    private void OpenFullAdEvent()
    {
        YandexGame.GameplayStop();
        print("Gameplay Start");
    }
    
    private void CloseFullAdEvent()
    {
        YandexGame.GameplayStart();
        print("Gameplay Start");
    }

    static event Action getRewardEvent;
    public static void ShowRewardVideo(Action action = null)
    {
        getRewardEvent = action;
        YandexGame.RewVideoShow(1);
        
        YandexGame.GameplayStop();
        print("Gameplay Stop");
    }

    static void GetReward(int id)
    {
        if (id != 1) return;
        
        getRewardEvent?.Invoke();
        
        YandexGame.GameplayStart();
        print("Gameplay Start");
    }
    #endregion
}

public enum GameState
{
    None, StartMenu, Game, Finish
}