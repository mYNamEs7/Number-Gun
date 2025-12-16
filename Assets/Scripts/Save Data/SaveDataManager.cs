using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager Instance { get; private set; }

    public static SaveGameData savesData;
    private DataHandler dataHandler;

    private List<ISaveData> saveDataObjects;

    public void Awake()
    {
        Instance = this;
        StartGame();
    }

    public void StartGame()
    {
        dataHandler = new DataHandler(Application.persistentDataPath, "GameData.json");
        saveDataObjects = FindAllSaveDataObjects();
        LoadGame();
    }

    public void NewGame()
    {
        savesData = new SaveGameData();
        dataHandler.Save(savesData);
    }

    public void LoadGame()
    {
        savesData = dataHandler.Load();

        if (savesData == null) NewGame();

        for (int i = 0; i < saveDataObjects.Count; i++) saveDataObjects[i].LoadData(savesData);
    }

    public static void SaveProgress() => Instance.SaveGame();
    public void SaveGame()
    {
        for (int i = 0; i < saveDataObjects.Count; i++) saveDataObjects[i].SaveData(ref savesData);
        
        dataHandler.Save(savesData);
    }

    List<ISaveData> FindAllSaveDataObjects()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<ISaveData>().ToList();
    }

}
