using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;
using UnityEngine.UI;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance { get; private set; }

    [SerializeField] private UIBlockUpgrade buyBlockBtn;
    [SerializeField] private Button ADBuyBlockBtn;
    [SerializeField] private Image ADBuyBlockImg;
    
    [NonSerialized] public BlockCell[] cells;

    private void Awake()
    {
        Instance = this;
        cells = GetComponentsInChildren<BlockCell>();
        
        UpdateBlockImage();
        ADBuyBlockBtn.onClick.AddListener(() =>
        {
            if (FirstFreeCell)
                GameManager.ShowRewardVideo(() =>
                {
                    var blockIndex = UpdateBlockImage();
                    SpawnBlock(blockIndex);
                });
        });
    }

    private int UpdateBlockImage()
    {
        var blockIndex = buyBlockBtn.blockIndex + 2 > GameData.Default.blockIcons.Length - 1
            ? GameData.Default.blockIcons.Length - 1
            : buyBlockBtn.blockIndex + 2;
        ADBuyBlockImg.sprite = GameData.Default.blockIcons[blockIndex];
        return blockIndex;
    }

    private void Start()
    {
        for (int i = 0; i < UsedBlocks.Length; i++)
        {
            SpawnBlock(UsedBlocks[i], cells[i]);
        }
    }

    private void OnEnable()
    {
        Card.OnCardSelected += CardOnOnCardSelected;
        Card.OnCardDeselected += CardOnOnCardDeselected;
    }

    private void OnDisable()
    {
        Card.OnCardSelected -= CardOnOnCardSelected;
        Card.OnCardDeselected -= CardOnOnCardDeselected;
    }

    private void CardOnOnCardSelected(Card card)
    {
        card.SetEnable();
    }

    private void CardOnOnCardDeselected(Card card, bool isMatch, List<GridCell> gridCells)
    {
        RemoveCardFromCell(card);
        if (!isMatch)
        {
            var cell = FirstFreeCell;
            cell?.SetBlock(card);
        }
    }

    public void RemoveCardFromCell(Card card)
    {
        if (!card.MyCell) return;
        card.MyCell.RemoveBlock();
    }

    public Card SpawnBlock(int blockIndex) => SpawnBlock(blockIndex, FirstFreeCell);
    public Card SpawnBlock(int blockIndex, int cellIndex) => SpawnBlock(blockIndex, cells.First(x => x.index == cellIndex));
    
    public Card SpawnBlockOnGrid(int blockIndex)
    {
        var spawnedBlock = Instantiate(GameData.Default.blocks[blockIndex], cells[0].thisTransform);
        spawnedBlock.isCollected = true;
        spawnedBlock.index = blockIndex;
        spawnedBlock.MyCell?.RemoveBlock();
        
        spawnedBlock.BaseUpgradeWindowOnOnStartUpgrade();
        return spawnedBlock;
    }

    public Card SpawnBlock(int blockIndex, BlockCell cell)
    {
        var spawnedBlock = Instantiate(GameData.Default.blocks[blockIndex], cell.thisTransform);
        spawnedBlock.index = blockIndex;
        spawnedBlock.isCollected = true;
        cell.SetBlock(spawnedBlock);
        
        spawnedBlock.BaseUpgradeWindowOnOnStartUpgrade();
        return spawnedBlock;
    }

    private int[] UsedBlocks => GameData.Blocks.Where(x => x != -1).ToArray();
    private bool BlocksIsEmpty => GameData.Blocks.All(x => x == -1);
    public BlockCell FirstFreeCell => cells.FirstOrDefault(x => x.IsFree && GameData.Blocks[cells.ToList().IndexOf(x)] == -1);
}
