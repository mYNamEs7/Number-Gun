using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDumpster : MonoBehaviour
{
    public void RemoveBlock(Card block)
    {
        BlockManager.Instance.RemoveCardFromCell(block);
        Destroy(block.gameObject);
        GameData.Default.AddCash(15 + 10 * block.index);
    }
}
