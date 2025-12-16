using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataHandler
{

    private string path;
    private string fileName;

    public DataHandler(string path, string fileName)
    {
        this.path = path;
        this.fileName = fileName;
    }

    public SaveGameData Load()
    {
        string fullPath = Path.Combine(path, fileName);
        //Create path for different OS

        SaveGameData loadedData = null;

        if (!File.Exists(fullPath)) return loadedData;
        //Can't find file

        try
        {
            FileStream stream = new FileStream(fullPath, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string data = reader.ReadToEnd();
            reader.Close();
            //Open file and read data

            loadedData = JsonUtility.FromJson<SaveGameData>(data);
            //Deserialize to SaveGameData
        }
        catch (Exception e)
        {
            Debug.LogError("Can't load data: " + e);
        }

        return loadedData;
    }

    public void Save(SaveGameData saveGameData)
    {
        string fullPath = Path.Combine(path, fileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string data = JsonUtility.ToJson(saveGameData, true);
            //Serialize to Json

            FileStream stream = new FileStream(fullPath, FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(data);
            writer.Close();
            //Create file and write data
        }
        catch (Exception e)
        {
            Debug.LogError("Can't save data: " + e);
        }
    }

}
