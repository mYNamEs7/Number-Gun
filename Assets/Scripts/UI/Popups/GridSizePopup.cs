using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridSizePopup : PopupUI
{
    //3x4 - 3x5 (01_0 - 02)
    //3x5 - 4x5 (01_5 - 02_3)
    //4x5 - 4x6 (01 - 02_0)
    //4x6 - 5x6 (01_3 - 02_1)
    //5x6 - 5x7 (01_4 - 02_2)
    //5x7 - 6x7 (01_2 - 02_5)
    //6x7 - 6x8 (01_1 - 02_4)
    //6x8 - 7x8 (01_6 - 02_6)
    [Header("Special Refs")]
    [SerializeField] private Image mainGridImg;
    [SerializeField] private Image addedGridImg;
    [SerializeField] private TMP_Text descriptionTxt;
    
    [Header("Params")]
    [SerializeField] private ImagePair[] imagePair;

    private ImagePair targetImgPair;
    private const string enDescription = "Upgrade Capacity \nFrom {0} to {1}";
    private const string ruDescription = "Увеличить Вместимость \nС {0} до {1}";

    public override void Init()
    {
        targetImgPair = imagePair.FirstOrDefault(x => x.curGridSize == GameData.GridSize);
        if (targetImgPair.Equals(default(ImagePair)))
        {
            Destroy();
            return;
        }
        
        base.Init();
        mainGridImg.sprite = targetImgPair.mainGrid;
        addedGridImg.sprite = targetImgPair.addedGrid;
        descriptionTxt.text = string.Format(GameData.Language == Language.EN ? enDescription : ruDescription, $"{targetImgPair.curGridSize.y}x{targetImgPair.curGridSize.x}", $"{targetImgPair.targetGridSize.y}x{targetImgPair.targetGridSize.x}");
    }

    protected override void OnReward() => GameData.GridSize = targetImgPair.targetGridSize;
    
    [Serializable]
    private struct ImagePair
    {
        public Vector2Int curGridSize;
        public Vector2Int targetGridSize;
        public Sprite mainGrid, addedGrid;
    }
}
