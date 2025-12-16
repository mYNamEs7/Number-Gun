using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Dreamteck;
using Grid;
using Lvls.Upgrades;
using TMPro;
using UI.Windows;
using UnityEngine;

public class Card : BulletTarget, IWeaponTarget
{
    public event Action OnHit; 
    public static event Action<Card> OnCardSelected; 
    public static event Action<Card, bool, List<GridCell>> OnCardDeselected;

    public bool isCollected;
    [SerializeField] CardData data;
    [SerializeField] int startLvl, maxLvl;
    protected int curLvl;
    public int CurLvl => curLvl;
    int fill;
    Tween hitTween, txtScaleTween, hitSoundTween;

    [NonSerialized] public bool canDrag;
    Vector3 upgradeIdlePos, dragOldPos;
    Tween returnPosAfterDragTween, returnRotAfterDragTween;

    [Header("Refs")]
    [SerializeField] TextMeshPro lvlFillCounterTxt;
    [SerializeField] private Transform filler;
    private MeshFiller meshFiller;
    [SerializeField] private Transform textFiller;
    [SerializeField] TextMeshPro[] upgradesTxt;
    [SerializeField] private Transform[] bounds;
    [SerializeField] public Vector2 scale;
    [SerializeField] private List<Transform> gunPoints = new List<Transform>();
    [SerializeField] private bool isSuperCard;

    public float addedValue;
    [SerializeField] private MeshRenderer visual;
    private Color startColor;
    
    Transform lvlFillCounterTxtTransform;
    private bool isUpgrade;
    [HideInInspector] public int id;
    private static int counter;

    private Vector3[] cellBounds;

    private Vector3 startScale = Vector3.one;
    private Vector3 endScale = Vector3.one;
    private Coroutine fillerRoutine;

    private Vector3[] startUpgradeTxtScales;
    public BlockCell MyCell => BlockManager.Instance.cells.FirstOrDefault(x => x.curBlock == this);
    [NonSerialized] public int index;
    [NonSerialized] public bool isEnable;

    [SerializeField] private Vector3 startOffset;

    public List<GridCell> lastCellArray = new List<GridCell>();

    public bool IsUsed { get; set; }
    public bool CanDrag { get; set; } = true;
    
    [SerializeField] List<Vector2Int> blockOffsets;
    public List<List<Vector2Int>> GetAllPossibleBlockCells(Vector2Int startPosition)
    {
        List<List<Vector2Int>> allBlockPositions = new List<List<Vector2Int>>();

        // Проходим по всем смещениям
        for (int i = 0; i < blockOffsets.Count; i++)
        {
            List<Vector2Int> blockCells = new List<Vector2Int>();

            // Смещение относительно текущей начальной клетки
            for (int j = 0; j < blockOffsets.Count; j++)
            {
                Vector2Int offset = blockOffsets[j] - blockOffsets[i];
                blockCells.Add(new Vector2Int(startPosition.x + offset.x, startPosition.y + offset.y));
            }

            allBlockPositions.Add(blockCells);
        }

        return allBlockPositions;
    }

    void OnEnable()
    {
        GameData.OnLanguageUpdate += Translate;
        
        Level.OnStartUpgrade += LevelOnOnStartUpgrade;
        Level.OnEndUpgrade += LevelOnOnEndUpgrade;
        
        BaseUpgradeWindow.OnStartUpgrade += BaseUpgradeWindowOnOnStartUpgrade;
    }

    void OnDisable()
    {
        GameData.OnLanguageUpdate -= Translate;
        
        Level.OnStartUpgrade -= LevelOnOnStartUpgrade;
        Level.OnEndUpgrade -= LevelOnOnEndUpgrade;
        
        BaseUpgradeWindow.OnStartUpgrade -= BaseUpgradeWindowOnOnStartUpgrade;
    }
    
    public void BaseUpgradeWindowOnOnStartUpgrade()
    {
        if (!isCollected) return;
        EnableUsage();
        LevelOnOnStartUpgrade();
        filler.gameObject.SetActive(true);
    }

    protected override void Awake()
    {
        base.Awake();

        id = ++counter;
        meshFiller = filler.GetComponentInChildren<MeshFiller>();
        
        curLvl = startLvl;
        lvlFillCounterTxtTransform = lvlFillCounterTxt.transform;

        UpdateFillCounter(true);

        if (filler.gameObject.activeInHierarchy)
            filler.gameObject.SetActive(false);
        
        if (GameData.LanguageEnable) Translate();

        if (isCollected)
        {
            Collect();
        }

        if (visual)
            startColor = visual.materials[0].color;
    }

    protected virtual int NeedBullets => data.lvls[curLvl].needBullets;
    protected virtual float GetValue(int upgrade) => data.upgrades[upgrade].upgrade[curLvl];
    protected virtual string GetUpgradeName(int upgrade) => GameData.Language == Language.RU ? data.upgrades[upgrade].ruName : (GameData.Language == Language.TR ? data.upgrades[upgrade].trName : data.upgrades[upgrade].enName);
    protected virtual List<WeaponUpgrade> GetWeaponUpgrades()
    {
        List<WeaponUpgrade> weaponUpgrades = new();
        data.upgrades.ForEach(x => weaponUpgrades.Add(x.upgrade));
        return weaponUpgrades;
    }

    public void Translate()
    {
        UpdateUpgradesName();
    }

    public void SetFill(float fill)
    {
        this.fill = Mathf.RoundToInt(NeedBullets * fill);
        filler.gameObject.SetActive(true);
        UpdateFillCounter();
    }

    public void SetDisable()
    {
        isEnable = false;
        visual.materials[0].color = Color.grey;
    }

    public void SetEnable()
    {
        isEnable = true;
        visual.materials[0].color = startColor;
    }

    void UpdateFillCounter(bool isAwake = false)
    {
        lvlFillCounterTxt.text = $"{fill}/{NeedBullets}";
        
        var value = (float)fill / NeedBullets;

        if (filler && !isAwake)
        {
            if (fillerRoutine != null) StopCoroutine(fillerRoutine);
            fillerRoutine = StartCoroutine(UpdateFillerScale(meshFiller.Value, Mathf.Clamp01(value), 0.2f));
        }

        if (textFiller)
        {
            var scale = textFiller.transform.localScale;
            scale.x = value;

            textFiller.transform.localScale = scale;
        }
    }

    private IEnumerator UpdateFillerScale(float startScale, float endScale, float time)
    {
        float currentTime = 0.0f;

        while (currentTime <= time)
        {
            meshFiller.HideUpperPart(Mathf.Lerp(startScale, endScale, currentTime / time));
            currentTime += Time.deltaTime;
            yield return null;
        }

        meshFiller.HideUpperPart(endScale);
    }
    
    void UpdateUpgradesName()
    {
        for (int i = 0; i < upgradesTxt.Length; i++)
        {
            string name = GetUpgradeName(i);
            upgradesTxt[i].text = string.Format(name, GetValue(i));
        }
    }

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        if (state == State.Collect) return;
        
        OnHit?.Invoke();
        fill += damage;
        if (!filler.gameObject.activeInHierarchy)
            filler.gameObject.SetActive(true);
        UpdateFillCounter();
        bool lvlUp = fill >= NeedBullets;

        hitTween.Kill(true);
        hitTween = thisTransform.DOPunchScale(Vector3.one * (GameData.Default.cardHitScale * (lvlUp ? 3 : 1)), GameData.Default.cardHitDuration, 0).SetEase(GameData.Default.cardHitEase);
        txtScaleTween.Kill(true);
        txtScaleTween = lvlFillCounterTxtTransform.DOPunchScale(Vector3.one * (GameData.Default.cardHitScale * (lvlUp ? 3 : 1)), GameData.Default.cardHitDuration, 0).SetEase(GameData.Default.cardHitEase);

        if (lvlUp) LvlUp();

        if (!hitSoundTween.IsActive())
        {
            hitSoundTween = DOTween.Sequence().SetDelay(0.05f).OnComplete(() => { });
        }
    }

    protected virtual void LvlUp()
    {
        if (state == State.Collect) return;

        Instantiate(GameData.Default.cardLvlUpParticles, thisTransform.position, Quaternion.Euler(-90, 0, 0));

        fill -= NeedBullets;
        curLvl++;
        if (curLvl >= maxLvl) Collect();
        
        if (filler.gameObject.activeInHierarchy)
            filler.gameObject.SetActive(false);
        UpdateFillCounter();
        Translate();
    }

    void Update()
    {
        if (state == State.Collect) return;
        if (thisTransform.position.z < PlayerController.Instance.thisTransform.position.z + 1.5f) Collect();
    }

    public void WeaponHit(Collider collider) => Collect();

    protected override void Collect()
    {
        if (state == State.Collect) return;
        base.Collect();

        if (TryGetComponent(out ObjectMover mover))
            Destroy(mover);

        if (!filler.gameObject.activeInHierarchy)
            filler.gameObject.SetActive(true);
        
        textFiller.parent.parent.gameObject.SetActive(false);
        if (fillerRoutine != null) StopCoroutine(fillerRoutine);
        StartCoroutine(FillFiller());
        
        if (isCollected) return;
        
        // thisTransform.localScale *= 0.425f;
        thisTransform.DOScale(Vector3.one * 0.85f, 0.33f);
        startScale = endScale = Vector3.one * 0.85f;
        // endScale = thisTransform.localScale;
        
        if (scale.y > 1f)
        {
            var count = (int)scale.y - 1;

            endScale.y += 0.05f * count;
        }
        
        if (scale.x > 1f)
        {
            var count = (int)scale.x - 1;

            endScale.x += 0.05f * count;
        }
        
        HPObstacle wall = GetComponentInChildren<HPObstacle>();
        if (wall) wall.thisTransform.parent = thisTransform.parent;
        
        Level.Instance.conveyorBelt.AddCard(this);
        SoundHolder.Default.PlayFromSoundPack("Whoosh");
    }

    private IEnumerator FillFiller()
    {
        meshFiller.HideUpperPart(1f);
        
        yield return new WaitForSeconds(0.2f);

        if (!filler.gameObject.activeInHierarchy)
            filler.gameObject.SetActive(true);

        meshFiller.HideUpperPart(1f);
    }

    public void HideText()
    {
        startUpgradeTxtScales = new Vector3[upgradesTxt.Length];
        for (int i = 0; i < upgradesTxt.Length; i++)
        {
            startUpgradeTxtScales[i] = upgradesTxt[i].transform.localScale;
            upgradesTxt[i].transform.localScale = Vector3.zero;
        }
    }

    public virtual void EnableUsage()
    {
        StartCoroutine(WaitForEnableUsages());
        for (int i = 0; i < upgradesTxt.Length; i++)
        {
            if (upgradesTxt[i].transform.localScale == Vector3.zero)
                upgradesTxt[i].transform.DOScale(startUpgradeTxtScales[i], 0.33f);
        }
    }

    private IEnumerator WaitForEnableUsages()
    {
        yield return new WaitForSeconds(0.33f);
        
        thisColliders.ForEach(x => x.enabled = true);
        canDrag = true;
    }
    
    public void DisableUsage()
    {
        thisColliders.ForEach(x => x.enabled = false);
        canDrag = false;
    }

    public void EnableCollect() => state = State.Collect;
    
    private void LevelOnOnEndUpgrade()
    {
        if (isSuperCard)
        {
            gunPoints.ForEach(x => x.localPosition = new Vector3(1.58f, x.localPosition.y, x.localPosition.z));
        }
        isUpgrade = false;
        lvlFillCounterTxt.transform.parent.gameObject.SetActive(true);

        thisTransform.localScale = startScale;

        if (IsUsed)
            AttachToGun();
        else
            Destroy(gameObject);
    }

    private void AttachToGun()
    {
        if (PlayerController.Instance.Weapons[0].detailsParent)
            thisTransform.SetParent(PlayerController.Instance.Weapons[0].detailsParent.thisTransform);
        
        var targetPos = thisTransform.localPosition;
        targetPos.x = 0f;
        thisTransform.localPosition = targetPos;
        
        thisTransform.GetChild(0).gameObject.SetActive(false);

        thisColliders.ForEach(x => x.enabled = true);
        // thisCollider.isTrigger = true;
        Destroy(this);
    }

    private void LevelOnOnStartUpgrade()
    {
        isUpgrade = true;
        lvlFillCounterTxt.transform.parent.gameObject.SetActive(false);

        // if (startScale != endScale)
        thisTransform.localScale = endScale;
    }

    public virtual Card StartDrag()
    {
        if (!canDrag) return null;
        
        OnCardSelected?.Invoke(this);
        
        if (!canDrag) return null;
        thisColliders.ForEach(x => x.enabled = false);

        Vector3 thisPos = thisTransform.position;
        returnPosAfterDragTween.Kill(true);
        returnRotAfterDragTween.Kill(true);

        // if (isCollected)
        //     myCell?.RemoveBlock();
        if (isCollected) upgradeIdlePos = thisTransform.position;
        if (isUpgrade)
        {
            upgradeIdlePos = thisTransform.position;
            isUpgrade = false;
        }
        dragOldPos = thisPos;
        thisTransform.position = thisPos;

        return this;
    }

    public void Drag()
    {
        Vector3 rot = (thisTransform.position - dragOldPos) * 3;
        thisTransform.rotation *= Quaternion.Euler(rot.x, rot.z, 0);
        dragOldPos = thisTransform.position;
        returnRotAfterDragTween.Kill();
        returnRotAfterDragTween = thisTransform.DORotate(UpgradeZone.CardSupineRotation, 0.5f);
    }

    public bool EndDrag()
    {
        cellBounds = Array.Empty<Vector3>();
        var cellArray = new List<GridCell>();

        if (isCollected && MyCell)
        {
            var pos = thisTransform.position;
            pos.y += 10f;

            if (Physics.Raycast(pos, Vector3.down, out var hit) && hit.transform.TryGetComponent(out BlockDumpster blockDumpster))
            {
                blockDumpster.RemoveBlock(this);
                return false;
            }

            Card secondCard = null;
            if (Physics.Raycast(pos, Vector3.down, out hit) && hit.transform.TryGetComponent(out BlockCell cell) && cell.curBlock != this && cell.curBlock)
                secondCard = cell.curBlock;
            else if (Physics.Raycast(pos, Vector3.down, out hit) && hit.transform.TryGetComponent(out Card card) && card != this)
                secondCard = card;

            if (secondCard)
            {
                if (MergeBlocks(secondCard))
                    return false;
            }
        }
        
        var isMatch = true;
        foreach (var bound in bounds)
        {
            var pos = bound.position;
            pos.y += 10f;
            
            if (!(Physics.Raycast(pos, Vector3.down, out var hit) &&
                  hit.transform.TryGetComponent(out GridCell cell) && cell.isPainted)) isMatch = false;
            else
            {
                cellBounds = cellBounds.Append(cell.transform.position).ToArray();
                cellArray.Add(cell);
            }
        }
        
        IsUsed = isMatch;

        if (IsUsed)
        {
            if (CanDrag)
                UpgradeGrid.CanPainting = true;
            
            var targetPos = (cellBounds[0] + cellBounds[1]) * 0.5f;
            targetPos.y = 0f;

            thisTransform.position = targetPos;
            
            lastCellArray = cellArray;
        }

        OnCardDeselected?.Invoke(this, IsUsed, cellArray);
        
        thisColliders.ForEach(x => x.enabled = true);
        
        if (IsUsed)
        {
            if (isCollected) return true;
            thisTransform.localEulerAngles = Vector3.right * 90f;
            return true;
        }

        if (isCollected)
        {
            returnPosAfterDragTween = thisTransform.DOMove(!MyCell ? upgradeIdlePos : MyCell.thisTransform.position + startOffset, 0.5f);
            if (!MyCell)
                OnCardDeselected?.Invoke(this, true, lastCellArray);
        }
        else
            returnPosAfterDragTween = thisTransform.DOMove(upgradeIdlePos, 0.5f);
        
        returnRotAfterDragTween.Kill();
        returnRotAfterDragTween = thisTransform.DORotate(UpgradeZone.CardSupineRotation, 0.5f);
        
        return false;
    }

    private bool MergeBlocks(Card secondCard)
    {
        if (index != secondCard.index || !secondCard.MyCell || index + 1 > GameData.Default.blocks.Length - 1) return false;

        if (!GameData.OpenedBlocks.Contains(index + 1))
        {
            GameData.OpenedBlocks = GameData.OpenedBlocks.Append(index + 1).ToArray();
            UIManager.Instance.ShowPopupUI(0.33f,  UIManager.blockPopupIndex);
        }
        
        var cellId = secondCard.MyCell.index;
        
        BlockManager.Instance.RemoveCardFromCell(this);
        BlockManager.Instance.RemoveCardFromCell(secondCard);
                
        BlockManager.Instance.SpawnBlock(index + 1, cellId);
                
        Destroy(gameObject);
        Destroy(secondCard.gameObject);
        return true;
    }

    public void AddPoints()
    {
        var playerPoints = PlayerController.Instance.Weapons[0].gunPoints;
        foreach (var point in gunPoints)
        {
            playerPoints.Add(point);
            var toRemove = playerPoints.Where(x => !gunPoints.Contains(x) && Mathf.Abs(point.transform.position.z - x.position.z) < 0.5f).ToArray();
            toRemove.ForEach(x => PlayerController.Instance.removedPoints.Push(x));
            toRemove.ForEach(x => playerPoints.Remove(x));
        }
        
        if (playerPoints[0].position.z <= 10)
            playerPoints.RemoveAt(0);
    }
    
    public void RemovePoints()
    {
        var playerPoints = PlayerController.Instance.Weapons[0].gunPoints;
        foreach (var point in gunPoints)
        {
            playerPoints.Remove(point);
            
            // if(playerPoints.Count <= 0)
            //     playerPoints.Add(PlayerController.Instance.Weapons[0].mainPoint.transform);
            
            var toReturn = PlayerController.Instance.removedPoints.Where(x => !gunPoints.Contains(x) && Mathf.Abs(point.transform.position.z - x.position.z) < 0.5f).ToList();
            playerPoints.AddRange(toReturn.Where(x => x.position.x == toReturn.Max(x => x.position.x) && !gunPoints.Contains(x)));
        }
        
        // if(playerPoints.Contains(PlayerController.Instance.Weapons[0].mainPoint.transform))
        //     playerPoints.ForEach(x =>
        //     {
        //         if (x != PlayerController.Instance.Weapons[0].mainPoint.transform) playerPoints.Remove(x);
        //     });
        
        // gunPoints.ForEach(x => playerPoints.Remove(x));
        if (playerPoints.Count > 0)
        {
            foreach (var playerPoint in playerPoints.ToList().Where(playerPoint => gunPoints.Contains(playerPoint)))
            {
                playerPoints.Remove(playerPoint);
            }
        } 
        
        if(playerPoints.Count <= 0)
            playerPoints.Add(PlayerController.Instance.Weapons[0].mainPoint.transform);
    }

    public List<WeaponUpgrade> GetUpgrades()
    {
        returnRotAfterDragTween.Kill();
        // thisTransform.DOScale(Vector3.zero, 0.2f).OnComplete(() => Destroy());
        return GetWeaponUpgrades();
    }
}
