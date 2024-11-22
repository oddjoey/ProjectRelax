using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryLogic : MonoBehaviour
{
    GameLogic game;
    [SerializeField] public int cellPixelSize = 32;
    [SerializeField] private Vector2Int inventorySize = new Vector2Int(10, 10);
    [SerializeField] private Vector2Int hotbarSize = new Vector2Int(10, 1);
    private GameObject items;

    public InventoryPanel inventoryGrid;
    public InventoryPanel hotbarGrid;
    public InventoryPanel heldItemPanel;

    private bool isCursorInInventory;
    private bool isHoldingItem;
    private bool isCursorInHotbar;

    private Vector2Int cellCursor;

    public InventoryItem currentHotbarItem;
    private Vector2Int heldItemCellPos;
    public InventoryItem.itemIDs lastHotbarItemType;
    public int lastHotbarItemIndex = -1;
    public GameObject holdingItem;
    public HeldItemLogic hotbarItemLogic;
    public void ClearPanel(ref InventoryPanel panel)
    {
        var items = GetItemsInCells(panel, 0, 0, panel.size.x, panel.size.y);

        foreach (var item in items)
            if (item != null && item.uiCellPosition == item.uiCellOrigin)
                Destroy(item.uiObject);

        for (int i = 0; i < panel.size.x; i++)
            for (int j = 0; j < panel.size.y; j++)
                panel.cells[i, j] = null;
    }
    private InventoryPanel GetCurrentPanel()
    {
        if (isCursorInInventory)
            return inventoryGrid;
        else if (isCursorInHotbar)
            return hotbarGrid;
        
        return null;
    }
    private InventoryItem GetItem(InventoryPanel panel, int cellX, int cellY)
    {
        if (cellX < 0 || cellX >= panel.size.x || cellY < 0 || cellY >= panel.size.y)
            return null;
        
        return panel.cells[cellX, cellY];
    }
    private void SetItemCellPosition(InventoryPanel panel, InventoryItem item, int cellX, int cellY)
    {
        var panelPosition = panel.GetPosition();
        item.rectTransform.position = new Vector3(panelPosition.x + (cellX * cellPixelSize), panelPosition.y + (cellY * cellPixelSize * -1), 0);
    }
    private bool AreCellsFree(InventoryPanel panel, int cellX, int cellY, int width, int height)
    {
        for (int i = cellX; i < cellX + width; i++)
            for (int j = cellY; j < cellY + height; j++)
                if (GetItem(panel, i, j) != null)
                    return false;

        return true;
    }
    public List<InventoryItem> GetItemsInCells(InventoryPanel panel, int cellX, int cellY, int width, int height)
    {
        List<InventoryItem> items = new List<InventoryItem>();
        for (int i = cellX; i < cellX + width; i++)
            for (int j = cellY; j < cellY + height; j++)
                items.Add(GetItem(panel, i, j));

        return items;
    }
    private bool IsPlacementValid(InventoryPanel panel, InventoryItem item, int cellX, int cellY)
    {
        if (cellX < 0 || cellY < 0)
            return false;

        if (item.cellSize.x > panel.size.x || item.cellSize.y > panel.size.y)
            return false;

        if (cellX + item.cellSize.x > panel.size.x || cellY + item.cellSize.y > panel.size.y)
            return false;

        if (!AreCellsFree(panel, cellX, cellY, item.cellSize.x, item.cellSize.y))
        {
            var items = GetItemsInCells(panel, cellX, cellY, item.cellSize.x, item.cellSize.y);
            foreach (var cellItem in items)
            {
                if (cellItem == null)
                {
                    //Debug.Log("Cell empty, continue");
                    continue;
                }
                else if (cellItem == item)
                {
                    //Debug.Log("Cell contains moving item, continue");
                    continue;
                }
                else if (cellItem != item)
                {
                    //Debug.Log("Cell is not empty, invalid palcement!");
                    return false;
                }
            }
        }

        return true;
    }
    private bool FindFirstOpenCells(ref Vector2Int openCellsPos, InventoryPanel panel, InventoryItem item)
    {
        for (int j = 0; j < panel.size.y; j++)
            for (int i = 0; i < panel.size.x; i++)
                if (IsPlacementValid(panel, item, i, j))
                {
                    openCellsPos = new Vector2Int(i, j);
                    return true;
                }
        
        return false;
    }
    public void AddItem(ref InventoryPanel panel, ref InventoryItem item, int cellX, int cellY)
    {
        for (int i = cellX; i < cellX + item.cellSize.x; i++)
            for (int j = cellY; j < cellY + item.cellSize.y; j++)
            {
                item.uiCellOrigin = new Vector2Int(cellX, cellY);
                item.uiCellPosition = new Vector2Int(i, j);
                panel.cells[i, j] = item;
            }

        item.rectTransform.SetParent(items.transform);
        SetItemCellPosition(panel, item, cellX, cellY);

        if (!game.UI.isInventoryMenuOpen && panel == inventoryGrid)
            item.uiObject.SetActive(false);
    }
    public bool AddItem(ref InventoryPanel panel, InventoryItem.itemIDs itemType)
    {
        InventoryItem item = new InventoryItem(itemType);

        Vector2Int openCells = new Vector2Int();
        if (!FindFirstOpenCells(ref openCells, panel, item))
            return false;

        AddItem(ref panel, ref item, openCells.x, openCells.y);

        return true;
    }
    public bool AddItem(InventoryItem.itemIDs itemType)
    {
        InventoryItem item = new InventoryItem(itemType);

        Vector2Int openCells = new Vector2Int();

        if (FindFirstOpenCells(ref openCells, hotbarGrid, item))
        {
            AddItem(ref hotbarGrid, ref item, openCells.x, openCells.y);
            return true;
        }
        else if (FindFirstOpenCells(ref openCells, inventoryGrid, item))
        {
            AddItem(ref inventoryGrid, ref item, openCells.x, openCells.y);
            return true;
        }

        return false;
    }
    private bool MoveItem(ref InventoryPanel oldPanel, ref InventoryPanel newPanel, int oldCellX, int oldCellY, int newCellX, int newCellY)
    {
        InventoryItem item = oldPanel.cells[oldCellX, oldCellY];

        if (item == null)
            return false;

        for (int i = oldCellX; i < oldCellX + item.cellSize.x; i++)
            for (int j = oldCellY; j < oldCellY + item.cellSize.y; j++)
                oldPanel.cells[i, j] = null;
        
        SetItemCellPosition(newPanel, item, newCellX, newCellY);

        for (int i = newCellX; i < newCellX + item.cellSize.x; i++)
            for (int j = newCellY; j < newCellY + item.cellSize.y; j++)
            {
                item.uiCellOrigin = new Vector2Int(newCellX, newCellY);
                item.uiCellPosition = new Vector2Int(i, j);
                newPanel.cells[i, j] = item;
            }
        return true;
    }
    public bool RemoveItem(ref InventoryPanel panel, int cellX, int cellY)
    {
        InventoryItem item = panel.cells[cellX, cellY];

        if (item == null)
            return false;

        for (int i = cellX; i < cellX + item.cellSize.x; i++)
            for (int j = cellY; j < cellY + item.cellSize.y; j++)
            {
                Destroy(panel.cells[i, j].rectTransform.gameObject);
                panel.cells[i, j] = null;
            }
        return true;
    }
    private bool MoveItemLogic()
    {
        var panel = GetCurrentPanel();

        if (!isHoldingItem)
        {
            if (panel == null)
                return false;

            var item = GetItem(panel, cellCursor.x, cellCursor.y);
            if ((isCursorInInventory || isCursorInHotbar) && game.inputs.action1 && item != null)
            {
                heldItemCellPos = item.uiCellOrigin;
                heldItemPanel = panel;
                isHoldingItem = true;
            }
        }
        else
        {
            ref var heldItem = ref heldItemPanel.cells[heldItemCellPos.x, heldItemCellPos.y];

            if (heldItem == null)
                return false;

            if (game.inputs.action1)
                heldItem.rectTransform.position = game.inputs.point;
            else
            {
                if (panel != null && IsPlacementValid(panel, heldItem, cellCursor.x, cellCursor.y))
                    MoveItem(ref heldItemPanel, ref panel, heldItemCellPos.x, heldItemCellPos.y, cellCursor.x, cellCursor.y);
                else
                    SetItemCellPosition(heldItemPanel, heldItem, heldItemCellPos.x, heldItemCellPos.y);

                heldItemCellPos = Vector2Int.zero;
                heldItemPanel = null;
                isHoldingItem = false;
            }
        }
        
        return true;
    }
    private void Calculate()
    {
        Vector2 inventoryCursorDelta = game.inputs.point - inventoryGrid.GetPosition();
        inventoryCursorDelta.y *= -1;
        Vector2 hotbarCursorDelta = game.inputs.point - hotbarGrid.GetPosition(); 
        hotbarCursorDelta.y *= -1;

        isCursorInInventory = inventoryCursorDelta.x >= 0 && inventoryCursorDelta.y >= 0 && inventoryCursorDelta.x < inventorySize.x * cellPixelSize && inventoryCursorDelta.y < inventorySize.y * cellPixelSize;
        isCursorInHotbar = hotbarCursorDelta.x >= 0 && hotbarCursorDelta.y >= 0 && hotbarCursorDelta.x < hotbarSize.x * cellPixelSize && hotbarCursorDelta.y < hotbarSize.y * cellPixelSize;

        cellCursor = Vector2Int.FloorToInt((isCursorInInventory ? inventoryCursorDelta : hotbarCursorDelta) / cellPixelSize);
    }
    private void SetupInventory()
    {
        inventoryGrid = new InventoryPanel("Inventory", inventorySize, cellPixelSize);
        hotbarGrid = new InventoryPanel("Hotbar", hotbarSize, cellPixelSize);
    }
    void Start()
    {
        game = GameLogic.instance;
        items = GameObject.Find("Items");
        
        SetupInventory();
    }
    void Update()
    {
        Calculate();
        MoveItemLogic();
    }
}
