using GameAnalyticsSDK;
using UnityEngine;
using YG;

public class GAAnalytics : MonoBehaviour
{
    void Awake()
    {
        GameAnalytics.Initialize();
        GameManager.OnStartGame += StartGame;
        GameManager.OnFinishEvent += (x) => CompleteGame();
    }

    void StartGame()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, (LevelManager.CompleteLevelCount + 1).ToString());
        YandexGame.GameplayStart();
        print("Gameplay Start");
    }

    void CompleteGame()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, LevelManager.CompleteLevelCount.ToString());
        YandexGame.GameplayStop();
        print("Gameplay Stop");
    }
    // void FailGame() => GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, (LevelManager.CompleteLevelCount + 1).ToString());
}