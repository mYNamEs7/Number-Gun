using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BlockPopup : PopupUI
{
    [Header("Special Refs")] 
    [SerializeField] private Image blockImg;

    private int blockIndex;
    
    public override void Init()
    {
        base.Init();
        blockIndex = GameData.OpenedBlocks.Last() - 1;
        blockImg.sprite = GameData.Default.blockIcons[blockIndex];
    }

    protected override void OnReward() => BlockManager.Instance.SpawnBlock(blockIndex);
}
