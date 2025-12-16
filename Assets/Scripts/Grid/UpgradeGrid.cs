using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck;
using UnityEngine;

namespace Grid
{
    public class UpgradeGrid : MonoBehaviour
    {
        public static event Action<int> OnCardCountChanged;

        [SerializeField] private bool isBased;
        [SerializeField] private BlockManager blockManager;
        
        private GridCell[] cells;
        private int cardCount;
        private readonly Stack<Card> lockedCards = new();
        
        private GridCell[,] grid;
        private readonly Dictionary<Card, List<GridCell>> occupiedCells = new();
        
        private int indexOffsetY;
        private int indexOffsetX;
        private int yMax, yMin;
        private Vector2Int maxGridSize = new(8, 7);
        private static Vector2Int gridSize;

        public GridCell[] Cells => cells;
        public static bool CanPainting { get; set; } = true;

        private void Awake()
        {
            cells = GetComponentsInChildren<GridCell>();
            gridSize = isBased ? GameData.SecondGridSize : GameData.GridSize;
            
            InitializeCells();
            InitializeGrid(gridSize);
            SetCellAvailable(0, 0);
        }

        private void Start()
        {
            if (isBased && GameData.GetGridCells() != null && GameData.GetGridCells().Length > 0)
            {
                var pos = GameData.GetGridCells();

                for (int i = 0; i < pos.Length; i++)
                {
                    var targetCells = cells.Where(cell => pos[i].Contains(cell.Position)).ToList();
                    var index = GameData.BlockIndexes[i];

                    var lastIndex = targetCells.Count - 1;
                    var firstIndex = 0;
                    switch (index)
                    {
                        case 3:
                            firstIndex = 1;
                            break;
                        case 4:
                            firstIndex = 2;
                            break;
                        case 6:
                            lastIndex = targetCells.Count - 2;
                            break;
                    }
                    
                    var targetBlock = blockManager.SpawnBlockOnGrid(index);
                    targetBlock.lastCellArray = targetCells;
                    
                    CardOnOnCardDeselected(targetBlock, true, targetCells);
                    
                    var targetPos = (targetCells[firstIndex].thisTransform.position + targetCells[lastIndex].thisTransform.position) * 0.5f;
                    targetPos.y = 0f;
                    targetBlock.thisTransform.position = targetPos;
                    
                    pos[i].ForEach(x => print(x));
                    
                    print("\n");
                }
            }

            if (isBased)
                StartCoroutine(UpdateBaseMultiplier());
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
            CanPainting = true;

            if (isBased)
            {
                GameData.SetGridCell(occupiedCells.Select(x => x.Value.ToArray()).ToArray());
                GameData.BlockIndexes = occupiedCells.Select(x => x.Key.index).ToArray();
            }
        }

        private void InitializeCells() => cells.ForEach(x => x.Init());

        public void InitializeGrid(Vector2Int size)
        {
            grid = new GridCell[size.x, size.y];
            indexOffsetX = 0;
            indexOffsetY = size.y / 2;
            
            yMax = indexOffsetY;
            yMin = -indexOffsetY;

            if (size.y == 4)
                yMax = indexOffsetY - 1;
            else if (size.y == 6)
            {
                yMin = -indexOffsetY + 1;
                yMax = indexOffsetY;
                indexOffsetY -= 1;
            }
            
            for (var x = 0; x < size.x; x++)
            {
                for (var y = yMin; y <= yMax; y++)
                {
                    var pos = new Vector2Int(x, y);
                    var targetCell = cells.First(x => x.Position == pos);
                    targetCell.SetActive();
                    SetGridCell(x, y, targetCell);
                }
            }
        }

        public void SetCellAvailable(int x, int y) => GetGridCell(x, y).isAvailable = true;

        public void ResetAvailableCells()
        {
            foreach (var cell in grid)
            {
                cell.isAvailable = false;
            }
        }
        
        public void ResetFreeCells()
        {
            foreach (var cell in grid)
            {
                cell.isFree = true;
            }
        }

        public GridCell[] GetAvailableCells()
        {
            var cells = Array.Empty<GridCell>();
            foreach (var cell in grid)
            {
                if (cell.isAvailable) 
                    cells = cells.Append(cell).ToArray();
            }

            return cells;
        }

        public bool IsCellFree(int x, int y) => GetGridCell(x, y).isFree;
        
        public void HighlightCells(List<Vector2Int> cellsToHighlight)
        {
            foreach (Vector2Int cellPos in cellsToHighlight)
            {
                GetGridCell(cellPos.x, cellPos.y).Paint();
            }
        }
        
        public void ResetHighlighting()
        {
            foreach (var cell in grid)
            {
                cell.Repaint();
            }
        }

        private GridCell GetGridCell(int x, int y) => grid[x + indexOffsetX, y + indexOffsetY];
        private void SetGridCell(int x, int y, GridCell value) => grid[x + indexOffsetX, y + indexOffsetY] = value;

        private void UpdateCells()
        {
            ResetAvailableCells();
            ResetFreeCells();

            if (occupiedCells.Values.Count == 0)
            {
                SetCellAvailable(0, 0);
                return;
            }

            if (isBased)
            {
                foreach (var key in occupiedCells.Keys)
                {
                    key.SetDisable();
                }

                for (var y = yMin; y <= yMax; y++)
                {
                    for (var x = 0; x < gridSize.x; x++)
                    {
                        var isContains = false;
                        foreach (var cells in occupiedCells)
                        {
                            if (cells.Value.Select(x => x.Position).Contains(new Vector2Int(x, y)))
                            {
                                isContains = true;
                                cells.Key.SetEnable();
                            }
                        }
                
                        if(!isContains)
                            break; 
                    }
                }
                
                foreach (var cells in occupiedCells.Where(x => x.Key.isEnable).Select(x => x.Value))
                {
                    foreach (var cell in cells)
                    {
                        cell.isFree = false;
                        
                        var cellPosition = cell.Position;
                    
                        if (cellPosition.x + 1 >= 0 && cellPosition.x + 1 < gridSize.x && cellPosition.y >= 0 - indexOffsetY && cellPosition.y + indexOffsetY < gridSize.y)
                            occupiedCells.FirstOrDefault(x => x.Value.Any(x => x.Position == GetGridCell(cellPosition.x + 1, cellPosition.y).Position)).Key?.SetEnable();

                        // if (cellPosition.x - 1 >= 0 && cellPosition.x - 1 < gridWidth && cellPosition.y >= 0 - indexOffsetY && cellPosition.y + indexOffsetY < gridHeight)
                        //     SetCellAvailable(cellPosition.x - 1, cellPosition.y);
                    
                        // if (cellPosition.x >= 0 && cellPosition.x < gridWidth && cellPosition.y + 1 >= 0 - indexOffsetY && cellPosition.y + 1 + indexOffsetY < gridHeight)
                        //     SetCellAvailable(cellPosition.x, cellPosition.y + 1);
                        //
                        // if (cellPosition.x >= 0 && cellPosition.x < gridWidth && cellPosition.y - 1 >= 0 - indexOffsetY && cellPosition.y - 1 + indexOffsetY < gridHeight)
                        //     SetCellAvailable(cellPosition.x, cellPosition.y - 1);
                    }
                }
            }
            else
            {
                foreach (var cells in occupiedCells.Values)
                {
                    foreach (var cell in cells)
                    {
                        cell.isFree = false;
                        
                        var cellPosition = cell.Position;
                    
                        if (cellPosition.x + 1 >= 0 && cellPosition.x + 1 < gridSize.x && cellPosition.y >= 0 - indexOffsetY && cellPosition.y + indexOffsetY < gridSize.y)
                            SetCellAvailable(cellPosition.x + 1, cellPosition.y);
                    
                        // if (cellPosition.x - 1 >= 0 && cellPosition.x - 1 < gridWidth && cellPosition.y >= 0 - indexOffsetY && cellPosition.y + indexOffsetY < gridHeight)
                        //     SetCellAvailable(cellPosition.x - 1, cellPosition.y);
                    
                        // if (cellPosition.x >= 0 && cellPosition.x < gridWidth && cellPosition.y + 1 >= 0 - indexOffsetY && cellPosition.y + 1 + indexOffsetY < gridHeight)
                        //     SetCellAvailable(cellPosition.x, cellPosition.y + 1);
                        //
                        // if (cellPosition.x >= 0 && cellPosition.x < gridWidth && cellPosition.y - 1 >= 0 - indexOffsetY && cellPosition.y - 1 + indexOffsetY < gridHeight)
                        //     SetCellAvailable(cellPosition.x, cellPosition.y - 1);
                    }
                }
            }
        }

        private bool CheckBlockPosition(Vector2Int cellPosition) => cellPosition.x + 1 >= 0 &&
                                                                  cellPosition.x + 1 < gridSize.x &&
                                                                  cellPosition.y >= 0 - indexOffsetY &&
                                                                  cellPosition.y + indexOffsetY < gridSize.y;

        private bool CheckBlockPosition(int cellPosX, int cellPosY) =>
            CheckBlockPosition(new Vector2Int(cellPosX, cellPosY));

        private IEnumerator UpdateBaseMultiplier()
        {
            yield return null;
            
            var startValue = 1f + occupiedCells.Where(x => x.Key.isEnable).Select(x => x.Key).Sum(card => card.addedValue);
            GameData.BaseMultiplier = startValue;
            print(startValue);
            PlayerController.Instance.MultiplyFireRate(1f);
        }
        
        private void CardOnOnCardDeselected(Card card, bool isMatch, List<GridCell> cellArray)
        {
            if (!CanPainting) return;
            
            occupiedCells.Remove(card);
            
            if (isMatch)
            {
                occupiedCells.Add(card, cellArray);
                OnCardCountChanged?.Invoke(CardCount);
            }

            if (isBased)
                StartCoroutine(UpdateBaseMultiplier());
            
            UpdateCells();
            
            ResetHighlighting();
            if (!isBased)
                BlockCards(isMatch);
        }

        private void BlockCards(bool isMatch)
        {
            if (CardCount > 1 && isMatch)
            {
                var maxPosX = occupiedCells.Keys.Select(x => x.thisTransform.position.x).Max();
                var toLockedCards = occupiedCells.Keys.Where(x => x.thisTransform.position.x < maxPosX).ToList();
                foreach (var targetCard in toLockedCards)
                {
                    if(!lockedCards.Contains(targetCard))
                        lockedCards.Push(targetCard);
                        
                    targetCard.canDrag = false;
                    cardCount = CardCount;
                }
            }
            else if (!isMatch && CardCount != cardCount && lockedCards.Count > 0)
            {
                var targetCard = lockedCards.Pop();
                targetCard.canDrag = true;
                
                cardCount = CardCount;
            }
        }
        
        List<List<Vector2Int>> CanPlaceBlock(Card card, Vector2Int startPosition)
        {
            var validBlockCells = new List<List<Vector2Int>>();
            
            var allBlockPositions = card.GetAllPossibleBlockCells(startPosition);

            foreach (var blockCells in allBlockPositions)
            {
                var canPlace = true;

                // Проверяем каждую позицию блока
                foreach (var cellPos in blockCells)
                {
                    if (cellPos.x < 0 || cellPos.x >= gridSize.x || cellPos.y < 0 - indexOffsetY || cellPos.y + indexOffsetY >= gridSize.y)
                    {
                        canPlace = false;
                        break;
                    }

                    if (!IsCellFree(cellPos.x, cellPos.y))
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                    validBlockCells.Add(blockCells);
            }

            return validBlockCells;
        }

        private void CardOnOnCardSelected(Card card)
        {
            if (!CanPainting) return;
            
            ResetHighlighting();
            occupiedCells.Remove(card);
            UpdateCells();
            if (isBased)
            {
                foreach (var cell in grid)
                {
                    cell.Paint(false);
                }
            }
            else
            {
                foreach (var cellPosition in GetAvailableCells().Select(x => x.Position))
                {
                    var targetCells = CanPlaceBlock(card, cellPosition);
                    if (targetCells.Count > 0)
                    {
                        foreach (var cells in targetCells)
                        {
                            HighlightCells(cells);
                        }
                    }
                }
            }
        }

        private int CardCount => occupiedCells.Count;
    }
}
