using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCell : MonoBehaviour
{
    [NonSerialized] public Transform thisTransform;

    public int index;

    [NonSerialized] public Card curBlock;

    public bool IsFree => !curBlock;

    private void Awake()
    {
        thisTransform = transform;
    }

    public void SetBlock(Card block)
    {
        curBlock = block;
        GameData.Blocks[index] = block.index;
    }

    public void RemoveBlock()
    {
        curBlock = null;
        GameData.Blocks[index] = -1;
    }
}
