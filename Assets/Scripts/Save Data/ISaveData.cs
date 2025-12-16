using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveData
{
    void LoadData(SaveGameData data) { }

    void SaveData(ref SaveGameData data) { }
}
