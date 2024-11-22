using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel
{
    GameLogic game;
    private RectTransform panel;
    public InventoryItem[,] cells;
    public Vector2Int size;
    public Vector2 GetPosition()
    {
        return panel.position;
    }
    public InventoryPanel(string panelName, Vector2Int gridSize, int cellPixelSize)
    {
        game = GameLogic.instance;
        
        size = gridSize;
        cells = new InventoryItem[size.x, size.y];

        panel = GameObject.Find(panelName).GetComponent<RectTransform>();
        panel.sizeDelta = size * cellPixelSize;
    }
}
