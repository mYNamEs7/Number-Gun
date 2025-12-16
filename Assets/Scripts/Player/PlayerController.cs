using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using MyInputManager;
using Dreamteck;
using Grid;
using UI.Windows;
using Unity.Mathematics;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    #region Singletone
    private static PlayerController _instance;
    public static PlayerController Instance { get => _instance; }
    public PlayerController() => _instance = this;
    #endregion

    public const float UpgradeAimOffset = 4.0f;
    public const float SpaceBetweenWeapons = 1.4f;
    readonly static Vector3 WeaponOffset = new Vector3(0, 1.0f, 3.12f);
    readonly static Dictionary<Language, string> AddYearsName = new Dictionary<Language, string> { { Language.RU, "{0} ЛЕТ" }, { Language.EN, "{0} YEARS" }, { Language.TR, "{0} YIL" } };
    readonly static Dictionary<Language, string> AddMonthsName = new Dictionary<Language, string> { { Language.RU, "+{0} МЕСЯЦЕВ" }, { Language.EN, "+{0} MONTHS" }, { Language.TR, "+{0} MONTHS" } };

    PlayerState state;
    public PlayerState State
    {
        get => state;
        set
        {
            if (value == state) return;
            state = value;
        }
    }

    public event Action<float> OnFireRateChanged;

    [SerializeField] public PlayerData data;
    [NonSerialized] public Transform thisTransform;
    float xInput, xPos;

    List<Weapon> weapons;
    public List<Weapon> Weapons => weapons;

    [NonSerialized] public Card curDragCard;
    Tween upgradeScaleTween;

    private Vector3 startPosition;
    private bool isLockedDrag;
    private Weapon removedWeapon;
    private Vector3 weaponStartPos;

    private Stack<float> multiplyCardLength = new Stack<float>();
    private Stack<float> targetAddedValue = new Stack<float>();

    private float incValue;
    private float oldIncValue;

    private float upgradeMultiplier;
    
    public Stack<Transform> removedPoints = new Stack<Transform>();
    
    public float FireRate { get; private set; }
    public float BulletSize { get; private set; }
    [NonSerialized] public Vector2 HorizontalClamp;

    private Camera cam;

    public void Init()
    {
        HorizontalClamp = data.horizontalClamp;
        BulletSize = 0;
        removedPoints = new Stack<Transform>();
        multiplyCardLength = new Stack<float>();
        targetAddedValue = new Stack<float>();
        incValue = 0;
        oldIncValue = 0;
        upgradeMultiplier = 1;
        
        RunnerController.OnControllEvent += Move;
        GameManager.OnStartMenu += StartMenu;
        GameManager.OnStartGame += StartGame;
        GameManager.OnFinishEvent += FinishGame;
        GameManager.OnRestartEvent += RestartGame;
        Level.OnStartUpgrade += StartUpgrade;
        Level.OnEndUpgrade += EndUpgrade;
        GameData.Default.GetUpgrade(UpgradeType.Years).OnUpgrade += AddYearsEvent;
        GameData.Default.GetUpgrade(UpgradeType.FireRate).OnUpgrade += UpdateFireRate;
        GameData.AddCashEvent += AddCashPopup;
        FinishZone.OnEnter += FinishZoneOnOnEnter;

        thisTransform = transform;
        cam = Camera.main;
        
        weapons = new();
        GetComponent<Rigidbody>().sleepThreshold = 0;
    }

    private void BaseUpgradeWindowOnOnEndUpgrade()
    {
        isLockedDrag = false;
        
        State = PlayerState.Idle;
        thisTransform.DOMove(startPosition, 0.5f);
        thisTransform.DORotate(new Vector3(0f, 0f, 0f), 0.5f);
        thisTransform.DOScale(Vector3.one * 1.25f, 0.5f);
        weapons.ForEach(x => x.EndUpgrade());
        
        InputManager.UnlockControll();
        InputManager.OnLockedDown -= TryDrag;
        InputManager.OnLockedUp -= EndDrag;
    }

    private void BaseUpgradeWindowOnOnStartUpgrade(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        isLockedDrag = true;
        startPosition = thisTransform.position;
        
        State = PlayerState.Upgrade;
        thisTransform.DOMove(position, 0.5f);
        thisTransform.DORotate(rotation, 0.5f);
        thisTransform.DOScale(scale, 0.5f);
        weapons.ForEach(x => x.StartUpgrade());
        
        InputManager.LockControll();
        InputManager.OnLockedDown += TryDrag;
        InputManager.OnLockedUp += EndDrag;
    }

    public void MultiplyFireRate(float multiplier) => OnFireRateChanged?.Invoke(FireRate *= multiplier);

    void StartMenu()
    {
        InputManager.DownEvent -= TryDrag;
        InputManager.UpEvent -= EndDrag;
        State = PlayerState.Idle;

        xPos = 0;
        weapons.ForEach(x => Destroy(x.gameObject));
        weapons.Clear();
        // while (thisTransform.childCount > 0) Destroy(thisTransform.GetChild(0).gameObject);

        int years = (int)GameData.Default.GetUpgrade(UpgradeType.Years).CurValue;
        print($"{years/50}");
        print($"{GameData.AddedYears}");
        print($"{years - GameData.AddedYears}");
        InstantiateWeapon(GameData.Default.weapons[Mathf.Min(years / 50, GameData.Default.weapons.Length - 1)]);
        weapons.ForEach(x => x.Upgrade(UpgradeType.Years, years % 50));

        weaponStartPos = weapons[0].thisTransform.localPosition;
        UpdateFireRate();
    }

    void StartGame()
    {
        State = PlayerState.Move;
        for (int i = 0; i < weapons.Count; i++) weapons[i].State = WeaponState.Shoot;
    }

    void FinishGame(bool win)
    {
        State = PlayerState.Finish;
        weapons.ForEach(x => x.State = WeaponState.Idle);
        if (!win) weapons.ForEach(x => x.Fall());
    }

    void RestartGame()
    {
        State = PlayerState.Idle;
        thisTransform.DORotate(new Vector3(0f, 0f, 0f), 0.5f);

        foreach (Transform child in thisTransform) Destroy(child.gameObject);
        
        RunnerController.OnControllEvent -= Move;
        GameManager.OnStartMenu -= StartMenu;
        GameManager.OnStartGame -= StartGame;
        GameManager.OnFinishEvent -= FinishGame;
        GameManager.OnRestartEvent -= RestartGame;
        Level.OnStartUpgrade -= StartUpgrade;
        Level.OnEndUpgrade -= EndUpgrade;
        GameData.Default.GetUpgrade(UpgradeType.Years).OnUpgrade -= AddYearsEvent;
        GameData.Default.GetUpgrade(UpgradeType.FireRate).OnUpgrade -= UpdateFireRate;
        GameData.AddCashEvent -= AddCashPopup;
        FinishZone.OnEnter -= FinishZoneOnOnEnter;
        
        Init();
        
        // Destroy(thisTransform.GetChild(thisTransform.childCount - 1).gameObject);
    }

    private void FinishZoneOnOnEnter()
    {
        weapons.ForEach(x =>
        {
            if (x.yearsTag)
                x.yearsTag.gameObject.SetActive(false);
        });
    }

    public void MoveSide()
    {
        thisTransform.DOMove(
                new Vector3(thisTransform.position.x + 1f, thisTransform.position.y, thisTransform.position.z - 2f),
                1f)
            .SetEase(Ease.OutFlash);
    }

    public Weapon InstantiateWeapon(Weapon weapon) => AddWeapon(Instantiate(weapon, thisTransform).Init());
    public Weapon AddWeapon(Weapon weapon, bool isReal = true)
    {
        float minDist = float.MaxValue;
        int nearWeapon = 0;
        if (isReal)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                float dist = Vector3.Distance(weapons[i].thisTransform.position, weapon.thisTransform.position);
                if (dist < minDist)
                {
                    nearWeapon = i;
                    minDist = dist;
                }
            }

            weapons.Insert(nearWeapon, weapon);
            weapon.thisTransform.localPosition = WeaponPos(nearWeapon);
            
            foreach (var weapon1 in weapons)
            {
                weapon1.UpdatePosX();
            }
        }
        else
        {
            if (!weapons.Contains(weapon))
                weapons.Add(weapon);
            
            weapon.State = WeaponState.Shoot;
        }
        
        weapons.ForEach(x => x.shootT = 0);
        return weapon;
    }

    public void YearsCenter()
    {
        var firstWeapon = weapons[0];
        
        var posA = firstWeapon.thisTransform.position;
        var posB = weapons.Last().thisTransform.position;
        var targetPos = (posA + posB) / 2;
        
        foreach (var weapon in weapons.Where(weapon => weapon != firstWeapon && weapon.yearsTag))
        {
            Destroy(weapon.yearsTag.gameObject);
        }
        
        // weapons.First(x => x.isReal).yearsTag.gameObject.SetActive(true);
        var tagPos = firstWeapon.yearsTag.transform.position;
        tagPos.x = targetPos.x;
        firstWeapon.yearsTag.transform.position = tagPos;
    }

    void StartUpgrade()
    {
        State = PlayerState.Upgrade;
        
        weapons[0].thisTransform.DOMove(Level.Instance.upgradeZone.PlayerPoint, 0.5f);
        weapons[0].thisTransform.DORotate(new Vector3(0f, 90f, 90f), 0.5f);

        if (weapons.Count > 1)
        {
            removedWeapon = weapons.Last();
            removedWeapon.thisTransform.DOMove(Level.Instance.upgradeZone.SecondGunPoint, 0.5f);
            KillWeapon(removedWeapon, true);
        }
        
        weapons.ForEach(x => x.StartUpgrade());
        InputManager.DownEvent += TryDrag;
        InputManager.UpEvent += EndDrag;
    }

    void EndUpgrade()
    {
        upgradeMultiplier = FireRate;
        
        State = PlayerState.Move;
        // weapons.First().thisTransform.DOMove(weaponPositions.First(), 0.5f);
        weapons[0].thisTransform.DORotate(new Vector3(0f, 0f, 0f), 0.5f);
        weapons[0].TransformRightGunPointZ();
        
        StartCoroutine(GunClone(removedWeapon));
        if (removedWeapon)
        {
            AddWeapon(removedWeapon, false);
            removedWeapon.TransformRightGunPointZ();
            removedWeapon = null;
        }

        foreach (var weapon in weapons)
        {
            var pos = weaponStartPos;
            pos.x = weapon.PosX;
            weapon.thisTransform.localPosition = pos;
        }
        
        weapons.ForEach(x => x.EndUpgrade());
        InputManager.DownEvent -= TryDrag;
        InputManager.UpEvent -= EndDrag;
    }

    private IEnumerator GunClone(bool doClone)
    {
        yield return new WaitForSeconds(0.1f);

        var toClone = weapons[0].detailsParent;
        toClone.points = weapons[0].gunPoints.ToArray();
        
        toClone.thisTransform.localScale = Vector3.one * 0.6f;
        foreach (Transform child in toClone.thisTransform)
        {
            child.localScale *= 1.19f;
        }

        var targetPos = toClone.thisTransform.localPosition;
        targetPos.y += weapons[0].GunPoint.y / 2;
        toClone.thisTransform.localPosition = targetPos;

        if (!doClone) yield break;
        
        var spawnedGun = Instantiate(toClone, toClone.thisTransform.position, toClone.thisTransform.rotation, weapons.Last().thisTransform);
        spawnedGun.thisTransform.SetLocalPositionAndRotation(targetPos, toClone.thisTransform.localRotation);

        var lastWeapon = weapons.Last();
        lastWeapon.data = weapons[0].data;
        lastWeapon.gunPoints = spawnedGun.points.ToList();
        lastWeapon.bulletsPool = weapons[0].bulletsPool;
        lastWeapon.bulletData = weapons[0].bulletData;
        lastWeapon.State = WeaponState.Shoot;
    }

    void Move(float x) => xInput = x;
    void TryDrag()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.TryGetComponent(out Card card))
        {
            if (!card.CanDrag)
            {
                UpgradeGrid.CanPainting = false;
            }
            
            curDragCard = card.StartDrag();
            if (curDragCard && curDragCard.IsUsed)
            {
                curDragCard.IsUsed = false;
                curDragCard.RemovePoints();
                List<WeaponUpgrade> upgrades = curDragCard.GetUpgrades();
                upgrades.Reverse();
                upgrades.ForEach(upgrade =>
                {
                    if (upgrade.type == UpgradeType.Multiplier)
                        Upgrade(UpgradeType.Multiplier, 1 / upgrade[curDragCard.CurLvl]);
                    else if (upgrade.type == UpgradeType.MultiShot)
                        Upgrade(UpgradeType.MultiShot, 1 / upgrade[curDragCard.CurLvl]);
                    else
                        Upgrade(upgrade.type, -upgrade[curDragCard.CurLvl]);
                });
                
                if (upgrades.Any(x => x.type == UpgradeType.Multiplier))
                    multiplyCardLength.Pop();
                else if(upgrades.Any(x => x.type == UpgradeType.Increase) && incValue != 0)
                    incValue -= upgrades.First(x => x.type == UpgradeType.Increase)[curDragCard.CurLvl];
            }
        }
    }
    void EndDrag()
    {
        if (curDragCard)
        {
            Card card = curDragCard;
            curDragCard = null;

            if (card.EndDrag())
            {
                // targetCells.ForEach(x => x.ReleaseUpgradeTarget());
                Level.Instance.upgradeZone.RemoveCard(card);
                int lvl = card.CurLvl;
                
                card.AddPoints();
                List<WeaponUpgrade> upgrades = card.GetUpgrades();
                upgrades.ForEach(upgrade => Upgrade(upgrade.type, upgrade[lvl], curCard: card));

                if (upgrades.Any(x => x.type == UpgradeType.Multiplier))
                {
                    multiplyCardLength.Push(card.scale.y);
                    incValue = 0;
                }
                else if (upgrades.Any(x => x.type == UpgradeType.Increase) && multiplyCardLength.Count > 0)
                    incValue += upgrades.First(x => x.type == UpgradeType.Increase)[lvl];
                else
                    incValue = 0;
            }
        }
    }

    public void Upgrade(UpgradeType type, float value, bool animate = true, List<Weapon> weapons = null, Card curCard = null)
    {
        weapons ??= this.weapons;
        switch (type)
        {
                case UpgradeType.Multiplier:
                {
                    var isInc = false;
                    
                    if (value < 1 && oldIncValue != 0)
                    {
                        value *= 1 / oldIncValue;
                        isInc = true;
                    }
                    
                    if (incValue != 0)
                    {
                        value *= incValue;
                        FireRate -= incValue;
                        oldIncValue = incValue;
                        incValue = 0;
                        isInc = true;
                    }
                    
                    if (curCard && multiplyCardLength.Count > 0 && !isInc)
                    {
                        var targetValue = FireRate * value - FireRate;
                        targetValue *= Mathf.Clamp01(curCard.scale.y / multiplyCardLength.Peek());
                        FireRate += targetValue;
                        targetAddedValue.Push(targetValue);
                        // FireRate *= value * Mathf.Clamp01(curCard.scale.y / multiplyCardLength);
                    }
                    else
                    {
                        if (targetAddedValue.Count > 0 && !isInc)
                        {
                            FireRate -= targetAddedValue.Pop();
                        }
                        else
                            FireRate *= value;
                    }

                    if (value < 1 && oldIncValue != 0)
                    {
                        FireRate += oldIncValue;
                        incValue = oldIncValue;
                        oldIncValue = 0;
                    }

                    OnFireRateChanged?.Invoke(FireRate);
                }
                break;
            case UpgradeType.Increase:
                {
                    FireRate += value;
                    weapons.ForEach(x => x.Upgrade(type, value));
                    OnFireRateChanged?.Invoke(FireRate);
                }
                break;
            case UpgradeType.GunAmount:
                {
                    if (Weapons.Count == 1)
                    {
                        OnFireRateChanged?.Invoke(FireRate * this.weapons.Count + 1);
                        weapons[0].Upgrade(type, value);
                    }
                }
                break;
            case UpgradeType.Cash:
                {
                    GameData.Default.AddCash((int)value);
                }
                break;
            case UpgradeType.Years:
                {
                    weapons.ForEach(x => x.Upgrade(UpgradeType.Years, value));
                    AddYearsPopup((int)value);
                    if (weapons.Count > 1)
                        YearsCenter();
                }
                break;
            case UpgradeType.Months:
                {
                    Popup(AddMonthsName[GameData.Language], (int)value);
                    weapons.ForEach(x => x.Upgrade(type, value));
                }
                break;
            case UpgradeType.Income:
                {
                    GameData.Default.cashRewardAddIncome += (int)value;
                }
                break;
            case UpgradeType.MultiShot:
                {
                    weapons.ForEach(x => x.Upgrade(type, value));
                    OnFireRateChanged?.Invoke(FireRate);
                }
                break;
            case UpgradeType.BulletSize:
                {
                    BulletSize = 2;
                    weapons.ForEach(x => x.Upgrade(type, value));
                    OnFireRateChanged?.Invoke(FireRate);
                }
                break;
            case UpgradeType.FireRate:
                {
                    FireRate += value * GameData.Default.fireRateUIToUpgradeMultiplier * upgradeMultiplier;
                    weapons.ForEach(x => x.Upgrade(type, value));
                    OnFireRateChanged?.Invoke(FireRate);
                    break;
                }
            default:
                {
                    weapons.ForEach(x => x.Upgrade(type, value));
                }
                break;
        }

        if (animate)
        {
            upgradeScaleTween.Kill(true);
            upgradeScaleTween = thisTransform.DOPunchScale(Vector3.one * 0.3f, 0.33f, 0);
        }
    }

    private void AddYearsEvent() => AddYears();

    void AddYears(int value = 5)
    {
        weapons.ForEach(x => x.Upgrade(UpgradeType.Years, value));
        AddYearsPopup(value);
    }

    public void UpdateFireRate()
    {
        FireRate = GameData.Default.fireRate * GameData.Default.GetUpgrade(UpgradeType.FireRate).CurValue + GameData.AddedFireRate - 0.1f * (LevelManager.CurrentLevel / 8);
        OnFireRateChanged?.Invoke(FireRate);
    }

    void AddYearsPopup(int value = 5)
    {
        if (value > 0) Popup("+" + AddYearsName[GameData.Language], value, isYearsPopup: true);
        else Popup(AddYearsName[GameData.Language], value, new Color(1, 0.1f, 0.1f), isYearsPopup: true);
    }
    void AddCashPopup(int value) { if (value > 0) Popup("+{0}<b>$", value); }
    public void Popup(string txt, int value, Color color = default, bool isYearsPopup = false) => Instantiate(GameData.Default.playerPopup, thisTransform.position + WeaponOffset + Vector3.up * 0.5f, Quaternion.identity, thisTransform).Init(string.Format(txt, value), color, isYearsPopup);

    public void Damage(int damage) => AddYears(-damage);
    public void Push(float pushStrength) => DOTween.To(() => thisTransform.localPosition, x => thisTransform.localPosition = x, thisTransform.localPosition + Vector3.back * pushStrength, 0.5f);
    public void PushLeft(float pushStrength) => DOTween.To(() => thisTransform.localPosition, x => thisTransform.localPosition = x, thisTransform.localPosition + Vector3.left * pushStrength, 0.1f);
    public void PushRight(float pushStrength) => DOTween.To(() => thisTransform.localPosition, x => thisTransform.localPosition = x, thisTransform.localPosition + Vector3.right * pushStrength, 0.1f);

    public void KillWeapon(Weapon weapon, bool isCard = false)
    {
        if (weapons.Count < 2) return;
        
        weapon.State = WeaponState.Idle;

        if(!isCard)
        {
            weapon.Fall();
            weapons.Remove(weapon);
        }
    }

    void Update()
    {
        for (int i = 0; i < weapons.Count; i++) weapons[i].CustomUpdate();

        if (state is PlayerState.Finish or PlayerState.Idle) return;

        switch (state)
        {
            case PlayerState.Move:
                float ratio = Mathf.Pow((9 / 16f) / (Screen.width / (float)Screen.height), data.horizontalSensetiveRatio);
                xPos = Mathf.Clamp(xPos + Mathf.Pow(Mathf.Abs(xInput), data.horizontalSensetivePow) * ratio * Mathf.Sign(xInput) * data.horizontalSensetive, HorizontalClamp.x, HorizontalClamp.y);
                // thisTransform.Translate(0, 0, data.forwardSpeed * Time.deltaTime);
                thisTransform.localPosition = new Vector3(Mathf.Lerp(thisTransform.localPosition.x, xPos, Time.deltaTime * data.horizontalSpeed), thisTransform.localPosition.y, thisTransform.localPosition.z + data.forwardSpeed * Time.deltaTime);
                break;
            case PlayerState.Upgrade:
                if (curDragCard)
                {
                    if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                    {
                        curDragCard.thisTransform.position = Vector3.Lerp(curDragCard.thisTransform.position, hit.point + Vector3.forward * 2, Time.deltaTime * 20);
                    }
                    curDragCard.Drag();
                }
                break;
        }
    }

    public void ClampPosX()
    {
        float ratio = Mathf.Pow((9 / 16f) / (Screen.width / (float)Screen.height), data.horizontalSensetiveRatio);
        xPos = Mathf.Clamp(xPos + Mathf.Pow(Mathf.Abs(xInput), data.horizontalSensetivePow) * ratio * Mathf.Sign(xInput) * data.horizontalSensetive, HorizontalClamp.x, HorizontalClamp.y);
    }

    Vector3 WeaponPos(int i) => WeaponOffset - weapons[i].GunPoint + Vector3.right * (SpaceBetweenWeapons * (i - Mathf.Max(weapons.Count - 1, 0) * 0.5f));

    GameObject oldCollisionObj;
    void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if (oldCollisionObj == obj) return;
        oldCollisionObj = obj;
        if (obj.TryGetComponent(out IWeaponTarget weaponTarget)) weaponTarget.WeaponHit(collision.contacts[0].thisCollider);
    }
}

public enum PlayerState
{
    Idle, Move, Upgrade, Finish
}