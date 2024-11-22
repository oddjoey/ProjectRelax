using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarLogic : MonoBehaviour
{
    GameLogic game;
    public int currentHotbarIndex = 0;
    private RectTransform hotbarHighlightGrid;
    void ScrollingHotbarLogic()
    {
        Vector3 selectedPosition = game.inventory.hotbarGrid.GetPosition();
        selectedPosition.x += currentHotbarIndex * game.inventory.cellPixelSize;

        hotbarHighlightGrid.position = selectedPosition;

        if (!game.UI.isCursorLocked)
            return;

        if (game.inputs.scroll.y < 0 && currentHotbarIndex < game.inventory.hotbarGrid.size.x - 1)
            currentHotbarIndex++;

        if (game.inputs.scroll.y > 0 && currentHotbarIndex > 0)
            currentHotbarIndex--;
    }
    // Start is called before the first frame update
    void Start()
    {
        game = GameLogic.instance;
        hotbarHighlightGrid = GameObject.Find("Selected Hotbar Item").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        ScrollingHotbarLogic();
    }
}
