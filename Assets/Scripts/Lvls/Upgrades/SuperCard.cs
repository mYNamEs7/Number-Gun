using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SuperCard : Card
{
    readonly static int[] HP = new int[] { 20, 250, 2500, 0 };
    readonly static Dictionary<Language, string>[] Name = new Dictionary<Language, string>[]
    {
        new Dictionary<Language, string> {{ Language.RU, "РЕДКАЯ КАРТА!" }, { Language.EN, "RARE CARD!" }, { Language.TR, "NADİR BİR KART!" }},
        new Dictionary<Language, string> {{ Language.RU, "ЭПИЧЕСКАЯ КАРТА!" }, { Language.EN, "EPIC CARD!" }, { Language.TR, "EPİK KART!" }},
        new Dictionary<Language, string> {{ Language.RU, "УНИКАЛЬНАЯ КАРТА!" }, { Language.EN, "UNIQUE CARD!" }, { Language.TR, "EŞSİZ KART!" }},
        new Dictionary<Language, string> {{ Language.RU, "ЛЕГЕНДАРНАЯ КАРТА!" }, { Language.EN, "LEGENDARY CARD!" }, { Language.TR, "EFSANEVİ KART!" }},
    };
    readonly static Dictionary<Language, string> WatchADName = new Dictionary<Language, string> { { Language.RU, "ПОСМОТРЕТЬ РЕКЛАМУ" }, { Language.EN, "WATCH AD" }, { Language.TR, "REKLAMI İZLE" } };

    [System.Serializable]
    class Upgrade
    {
        [System.Serializable]
        public class Lvl
        {
            public float value;
            public string ruName, enName, trName;
        }
        public UpgradeType type;
        public Lvl[] lvls;

        public float this[int lvl] => lvl < lvls.Length ? lvls[lvl].value : lvls[lvls.Length - 1].value;
    }

    [SerializeField] ParticleSystem particles;
    [SerializeField] SpriteRenderer background, glow;
    [SerializeField] Color[] colors;
    [SerializeField] GameObject adIcon;
    [SerializeField] GameObject adTimer;
    [SerializeField] Transform adTimerSlider;
    ParticleSystem.MainModule mainModule;
    public bool unlock = true;
    Tween adTimerTween;

    [Space, SerializeField] Upgrade[] upgrades;

    protected override void Awake()
    {
        base.Awake();
        mainModule = particles.main;
        if (LevelManager.Default.CurrentLevelIndex > 3) unlock = false;
    }

    protected override void LvlUp()
    {
        base.LvlUp();
        mainModule.startColor = colors[curLvl];
        background.color = colors[curLvl];
        glow.color = colors[curLvl];
    }

    int MaxLvl(int upgrade) => Mathf.Min(CurLvl, upgrades[upgrade].lvls.Length - 1);
    protected override int NeedBullets => HP[CurLvl];
    protected override float GetValue(int upgrade) => upgrades[upgrade][curLvl];
    protected override string GetUpgradeName(int upgrade) => GameData.Language == Language.RU ? upgrades[upgrade].lvls[MaxLvl(upgrade)].ruName : (GameData.Language == Language.TR ? upgrades[upgrade].lvls[MaxLvl(upgrade)].trName : upgrades[upgrade].lvls[MaxLvl(upgrade)].enName);
    protected override List<WeaponUpgrade> GetWeaponUpgrades()
    {
        List<WeaponUpgrade> weaponUpgrades = new();
        for (int i = 0; i < upgrades.Length; i++)
        {
            float[] value = new float[upgrades[i].lvls.Length];
            for (int j = 0; j < value.Length; j++) value[j] = upgrades[i][j];
            weaponUpgrades.Add(new WeaponUpgrade { type = upgrades[i].type, value = value });
        }
        return weaponUpgrades;
    }

    public override void EnableUsage()
    {
        base.EnableUsage();
        if (!unlock)
        {
            adIcon.SetActive(true);
            adTimer.gameObject.SetActive(true);
            adTimerTween = adTimerSlider.DOLocalMoveX(-1, 25).SetEase(Ease.Linear);
        }
    }

    public override Card StartDrag()
    {
        if (!unlock)
        {
            GameManager.ShowRewardVideo(Unlock);
            return null;
        }
        return base.StartDrag();
    }

    void Unlock()
    {
        unlock = true;
        adIcon.SetActive(false);
        adTimerTween.Kill();
        adTimer.gameObject.SetActive(false);
    }

    public override void Destroy()
    {
        adTimerTween.Kill();
        base.Destroy();
    }
}
