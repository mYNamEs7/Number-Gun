using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Grid;
using UnityEngine;
using YG;

[CreateAssetMenu(menuName = "Data/GameData")]
public class GameData : DataHolder
{
    #region Singleton
    private static GameData _default;
    public static GameData Default => _default;
    #endregion

    public static bool LanguageEnable;
    public static event Action OnLanguageUpdate;
    readonly static Dictionary<string, Language> LanguageType = new Dictionary<string, Language> { { "ru", Language.RU }, { "en", Language.EN }, { "tr", Language.TR } };

    public override void Init()
    {
        _default = this;
        curUICash = Cash;
        GameManager.OnRestartEvent += () => cashRewardMultiplier = 1;
        GameManager.OnRestartEvent += () => cashRewardAddIncome = 0;

        if (YandexGame.SDKEnabled) YGInit();
        else YandexGame.GetDataEvent += YGInit;
    }

    void YGInit()
    {
#if UNITY_EDITOR
        Language = EditorLanguage;
#else
        Language = LanguageType[YandexGame.EnvironmentData.language];
#endif
        OnLanguageUpdate?.Invoke();
        LanguageEnable = true;
    }

    public Language EditorLanguage;

    [Header("Balance")] 
    public float[] fireRateByYears;
    
    public float fireRateUIToUpgradeMultiplier;
    public float fireRangeUIToUpgradeMultiplier;

    [Space(10)]
    public float fireRate;
    public float fireRange;
    public float bulletSpeed;
    public float bulletSize;

    [Space(10)]
    public int startCashReward;
    [NonSerialized] public int cashRewardAddIncome = 0;
    [NonSerialized] public int cashRewardMultiplier = 1;

    [Serializable]
    public class ReceiveNewCard
    {
#if UNITY_EDITOR
        public string name;
#endif
        public int lvlReceive;
    }

    [Space(10)]
    public ReceiveNewCard[] newCards;

    [Serializable]
    public class WeaponUI
    {
        public string name, nameRU, nameTR;
        public Sprite icon;
        public Sprite inactiveIcon;
        public Sprite goldenIcon;
    }

    [Space(15), Header("Visual")]
    public UITransition transition;
    public GameObject cashUI, keyUI;
    public GameObject upgradeArrowUI;
    public YearsTag yearsTag;
    public Material gunsMaterial;
    public Material goldenGunsMaterial;
    public Popup playerPopup;
    public WeaponUI[] weaponsUI;
    public Sprite[] handsUI;
    public static Language Language = Language.EN;

    [Space(10)]
    public ParticleSystem bulletDestroyParticles;
    public ParticleSystem cardLvlUpParticles;
    public ParticleSystem cashParticles;
    public GameObject cashGateParticles;
    public ParticleSystem teleportParticles;
    public ParticleSystem upgradeTargetParticles;

    [Space(10)]
    public float cardHitScale;
    public float cardHitDuration;
    public Ease cardHitEase;
    public GameObject cardUpgradeAim;

    [Space(15), Header("Refs")] 
    public Card[] blocks;
    public Sprite[] blockIcons;
    public Weapon[] weapons;

    public Hand[] hands;
    public static int[] AvailableHandIds { get { return YandexGame.savesData.availableHandIds; } set { YandexGame.savesData.availableHandIds = value; YandexGame.SaveProgress(); } }
    public static int CurHandId
    {
        get { return YandexGame.savesData.curHandId; } 
        set
        {
            if (!AvailableHandIds.Contains(value))
                AvailableHandIds = AvailableHandIds.Append(value).ToArray();
            
            YandexGame.savesData.curHandId = value;
            YandexGame.SaveProgress();
        }
    }
    public static int FillHandId { get { return YandexGame.savesData.fillHandId; } set { YandexGame.savesData.fillHandId = value; YandexGame.SaveProgress(); } }
    public static int FillHandPercent { get { return YandexGame.savesData.fillHandPercent; } set { YandexGame.savesData.fillHandPercent = value; YandexGame.SaveProgress(); } }

    public List<Upgrade> upgrades;
    public Upgrade GetUpgrade(UpgradeType type) => upgrades.Find(x => x.type == type);

    public Cash cash;

    public Material[] podiumMaterials;

    public PopupUI[] popups;
    
    //Added Values
    public static float GoldenGunFireRate { get { return YandexGame.savesData.goldenGunFireRate; } set { YandexGame.savesData.goldenGunFireRate = value; YandexGame.SaveProgress(); } }
    public static float GoldenGunFireRange { get { return YandexGame.savesData.goldenGunFireRange; } set { YandexGame.savesData.goldenGunFireRange = value; YandexGame.SaveProgress(); } }
    public static float HandIncome { get { return YandexGame.savesData.handIncome; } set { YandexGame.savesData.handIncome = value; YandexGame.SaveProgress(); } }
    public static float HandFireRate { get { return YandexGame.savesData.handFireRate; } set { YandexGame.savesData.handFireRate = value; YandexGame.SaveProgress(); } }
    public static float HandFireRange { get { return YandexGame.savesData.handFireRange; } set { YandexGame.savesData.handFireRange = value; YandexGame.SaveProgress(); } }
    
    //Permanently
    public static float AddedFireRate => GoldenGunFireRate + HandFireRate;
    public static float AddedFireRange => GoldenGunFireRange + HandFireRange;
    public static float AddedIncome => HandIncome;
    public static bool[] GoldenWeapons { get { return YandexGame.savesData.goldenWeapons; } set { YandexGame.savesData.goldenWeapons = value; YandexGame.SaveProgress(); } }
    public static Vector2Int GridSize { get { return YandexGame.savesData.gridSize; } set { YandexGame.savesData.gridSize = value; YandexGame.SaveProgress(); } }
    public static Vector2Int SecondGridSize { get { return YandexGame.savesData.secondGridSize; } set { YandexGame.savesData.secondGridSize = value; YandexGame.SaveProgress(); } }
    public static int IncreaseStartIndex { get { return YandexGame.savesData.increaseStartIndex; } set { YandexGame.savesData.increaseStartIndex = value; YandexGame.SaveProgress(); } }
    public static int MultiplierStartIndex { get { return YandexGame.savesData.multiplierStartIndex; } set { YandexGame.savesData.multiplierStartIndex = value; YandexGame.SaveProgress(); } }
    public static int YearGateCapacity { get { return YandexGame.savesData.yearGateCapacity; } set { YandexGame.savesData.yearGateCapacity = value; YandexGame.SaveProgress(); } }
    
    //Base Multiplier
    public static int[] Blocks { get { return YandexGame.savesData.blocks; } set { YandexGame.savesData.blocks = value; YandexGame.SaveProgress(); } }
    private static string[] GridCells { get { return YandexGame.savesData.gridCells; } set { YandexGame.savesData.gridCells = value; YandexGame.SaveProgress(); } }
    public static int[] BlockIndexes { get { return YandexGame.savesData.blockIndexes; } set { YandexGame.savesData.blockIndexes = value; YandexGame.SaveProgress(); } }
    public static int[] OpenedBlocks { get { return YandexGame.savesData.openedBlocks; } set { YandexGame.savesData.openedBlocks = value; YandexGame.SaveProgress(); } }
    public static float BaseMultiplier { get { return YandexGame.savesData.baseMultiplier; } set { YandexGame.savesData.baseMultiplier = value; YandexGame.SaveProgress(); } }
    
    //Shop
    public static string[] MoneyTimers { get { return YandexGame.savesData.moneyTimers; } set { YandexGame.savesData.moneyTimers = value; YandexGame.SaveProgress(); } }
    public static int[] GunItems { get { return YandexGame.savesData.gunItems; } set { YandexGame.savesData.gunItems = value; YandexGame.SaveProgress(); } }
    public static int AddedYears { get { return YandexGame.savesData.addedYears; } set { YandexGame.savesData.addedYears = value; YandexGame.SaveProgress(); OnAddedYearsChanged?.Invoke(value); } }
    public static event Action<int> OnAddedYearsChanged;

    public static Vector2Int[][] GetGridCells()
    {
        return GridCells?.Select(row => row.Split(',')
                .Select(cell =>
                {
                    var parts = cell.Split(':');
                    return new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]));
                }).ToArray())
            .ToArray();
    }

    public static void SetGridCell(GridCell[][] cells)
    {
        GridCells = cells
            .Select(row => string.Join(",", row.Select(cell => $"{cell.Position.x}:{cell.Position.y}").ToArray()))
            .ToArray();
    }


    private int curUICash;
    public static event Action UpdateCashEvent;
    public static event Action<int> AddCashEvent;

    public void UpdateCash()
    {
        UIMoney.Instance.SetCash(curUICash);
        UpdateCashEvent?.Invoke();
    }

    public static int Cash { get { return YandexGame.savesData.Cash; } set { YandexGame.savesData.Cash = value; YandexGame.SaveProgress(); } }

    public void AddCash(int count)
    {
        Cash += count;
        curUICash += count;
        AddCashEvent?.Invoke(count);
        UpdateCash();
    }

    public void AddUICash(int count)
    {
        curUICash += count;
        AddCashEvent?.Invoke(count);
        UpdateCash();
    }

    public void UpdateUICash()
    {
        curUICash = Cash;
        UpdateCash();
    }

    public bool PayCash(int cost)
    {
        if (!TryPayUICash(cost)) return false;
        PayCash();
        UpdateCash();

        return true;
    }

    public void PayCash() => Cash = curUICash;

    public bool TryPayUICash(int cost)
    {
        if (cost > curUICash) return false;

        curUICash -= cost;
        return true;
    }


    public static event Action UpdateKeysEvent;

    public void UpdateKeys()
    {
        UIMoney.Instance.SetKeys(Keys);
        UpdateKeysEvent?.Invoke();
    }

    public static int Keys { get { return YandexGame.savesData.Keys; } set { YandexGame.savesData.Keys = value; YandexGame.SaveProgress(); } }

    public void AddKeys(int count)
    {
        Keys += count;
        UpdateKeys();
    }

    public bool TryPayKeys(int cost)
    {
        if (cost > Keys) return false;

        AddKeys(-cost);
        return true;
    }
}

public enum Language
{
    RU, EN, TR
}