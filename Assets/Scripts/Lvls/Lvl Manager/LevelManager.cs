using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using YG;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    #region Singletone
    private static LevelManager _default;
    public static LevelManager Default { get => _default; }
    public LevelManager() => _default = this;
    #endregion

    public bool editorMode = false;
    public static int CurrentLevel { get { return YandexGame.savesData.CurrentLevel + 1; } set { YandexGame.savesData.CurrentLevel = value; YandexGame.SaveProgress(); } }
    public static int CompleteLevelCount { get { return YandexGame.savesData.CompleteLevelCount; } set { YandexGame.savesData.CompleteLevelCount = value; YandexGame.SaveProgress(); } }
    public int CurrentLevelIndex;
    [SerializeField] LvlsList levels;
    public List<LvlsList.Lvl> Levels => levels.lvls;

    public event Action OnLevelStarted;


    public void Init()
    {
#if !UNITY_EDITOR
        editorMode = false;
#endif
        if (!editorMode) SelectLevel(YandexGame.savesData.LastLevelIndex, true);

        if (YandexGame.savesData.LastLevelIndex != YandexGame.savesData.CurrentLevel)
        {
            YandexGame.savesData.CurrentAttempt = 0;
            YandexGame.SaveProgress();
        }
    }

    private void OnDestroy()
    {
        YandexGame.savesData.LastLevelIndex = CurrentLevelIndex;
        YandexGame.SaveProgress();
    }

    private void OnApplicationQuit()
    {
        YandexGame.savesData.LastLevelIndex = CurrentLevelIndex;
        YandexGame.SaveProgress();
    }


    public void StartLevel()
    {
        OnLevelStarted?.Invoke();
    }

    public void RestartLevel()
    {
        SelectLevel(CurrentLevelIndex, false);
    }

    public void NextLevel()
    {
        if (!editorMode) CurrentLevel++;
        SelectLevel(CurrentLevelIndex + 1);
    }

    public void SelectLevel(int levelIndex, bool indexCheck = true)
    {
        if (indexCheck)
            levelIndex = GetCorrectedIndex(levelIndex);

        if (Levels[levelIndex] == null)
        {
            Debug.Log("<color=red>There is no prefab attached!</color>");
            return;
        }

        var level = Levels[levelIndex];

        if (level.data)
        {
            SelLevelParams(level.data);
            CurrentLevelIndex = levelIndex;
        }
    }

    public void PrevLevel() =>
        SelectLevel(CurrentLevelIndex - 1);

    private int GetCorrectedIndex(int levelIndex)
    {
        if (editorMode)
            return levelIndex > Levels.Count - 1 || levelIndex <= 0 ? 0 : levelIndex;
        else
        {
            int levelId = YandexGame.savesData.CurrentLevel;
            if (levelId > Levels.Count - 1)
            {
                if (levels.randomizedLvls)
                {
                    List<int> lvls = Enumerable.Range(0, levels.lvls.Count).ToList();
                    lvls.RemoveAt(CurrentLevelIndex);
                    return lvls[UnityEngine.Random.Range(0, lvls.Count)];
                }
                else return (levelIndex % (levels.lvls.Count - 2)) + 2;
            }
            return levelId;
        }
    }

    private void SelLevelParams(Level level)
    {
        if (level)
        {
            level.Init();
            ClearChilds();
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Instantiate(level, transform);
            }
            else
            {
                PrefabUtility.InstantiatePrefab(level, transform);
            }
            foreach (IEditorModeSpawn child in GetComponentsInChildren<IEditorModeSpawn>())
                child.EditorModeSpawn();
#else
            Instantiate(level, transform);
#endif
        }
    }

    private void ClearChilds()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject destroyObject = transform.GetChild(i).gameObject;
            DestroyImmediate(destroyObject);
        }
    }
}