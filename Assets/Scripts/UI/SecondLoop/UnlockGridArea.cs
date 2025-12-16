using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UnlockGridArea : MonoBehaviour
{
    [NonSerialized] public Transform thisTransform;

    [Header("Refs")] 
    [SerializeField] private UpgradeGrid grid;
    
    [Header("Params")]
    [SerializeField] private Areas[] areas;

    private Areas targetArea;
    private GridArea curArea;

    private void Awake()
    {
        thisTransform = transform;
    }

    private void Start()
    {
        SpawnArea();
    }

    public void RespawnArea()
    {
        if (curArea) Destroy(curArea.gameObject);
        SpawnArea();
    }

    private void SpawnArea()
    {
        grid.InitializeGrid(GameData.SecondGridSize);
        if (!UpdateTargetArea()) return;
        
        curArea = Instantiate(targetArea.prefab, thisTransform);
        curArea.Init(targetArea.targetGridSize, this);
    }

    private bool UpdateTargetArea()
    {
        targetArea = areas.FirstOrDefault(x => x.curGridSize == GameData.SecondGridSize);
        return !targetArea.Equals(default(Areas));
    }
    
    [Serializable]
    private struct Areas
    {
        public Vector2Int curGridSize;
        public Vector2Int targetGridSize;
        public GridArea prefab;
    }
}
