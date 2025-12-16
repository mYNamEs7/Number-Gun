
using System;
using System.Collections.Generic;
using Grid;
using UnityEngine;
using UnityEngine.Serialization;

namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;

        public int Cash = 10000, Keys;
        
        public int CurrentLevel;
        public int CompleteLevelCount;
        public int LastLevelIndex;
        public int CurrentAttempt;
        public float Record;
        
        public bool Music, SFX;
        
        public int[] UpgradesLvl = new int[] { -1, -1, -1, -1, -1 };
        
        public int CurChestCardPack;

        public int curHandId;
        public int[] availableHandIds = new[] { 0 };

        public int fillHandId;
        public int fillHandPercent;
        
        //Added Values
        public float goldenGunFireRate;
        public float goldenGunFireRange;
        public float handIncome;
        public float handFireRate;
        public float handFireRange;
        
        //Permanently
        public bool[] goldenWeapons = new bool[33];
        public Vector2Int gridSize = new(4, 3);
        public Vector2Int secondGridSize = new(4, 3);
        public int increaseStartIndex;
        public int multiplierStartIndex;
        public int yearGateCapacity;
        
        //Base Multiplier
        public int[] blocks = new int[] { -1, -1, -1 };
        public string[] gridCells;
        public int[] blockIndexes = Array.Empty<int>();
        public int[] openedBlocks = new[] { 0, 1 };
        public float baseMultiplier = 1;
        
        //Shop
        public string[] moneyTimers = Array.Empty<string>();
        public int[] gunItems = Array.Empty<int>();
        public int addedYears;
        
        // Тестовые сохранения для демо сцены
        // Можно удалить этот код, но тогда удалите и демо (папка Example)
        public int money = 1;                       // Можно задать полям значения по умолчанию
        public string newPlayerName = "Hello!";
        public bool[] openLevels = new bool[3];

        // Ваши сохранения

        // ...

        // Поля (сохранения) можно удалять и создавать новые. При обновлении игры сохранения ломаться не должны


        // Вы можете выполнить какие то действия при загрузке сохранений
        public SavesYG()
        {
            // Допустим, задать значения по умолчанию для отдельных элементов массива

            openLevels[1] = true;
        }
    }
}
