using UnityEngine;

public class InventoryItem
{
    public enum itemIDs
    {
        knife,
        healthPotion,
        pistol
    }

    public RectTransform rectTransform;
    public GameObject uiObject;
    public GameObject heldObject;
    public Vector3 localPosition;
    public Vector3 localRotation;
    public Vector2Int cellSize;
    public Vector2Int uiCellOrigin;
    public Vector2Int uiCellPosition;
    public itemIDs type;
    public float damage;
    public string name;
    private void InitializeOnType()
    {
        switch (type)
        {
            case itemIDs.knife:
                cellSize = new Vector2Int(1, 1);
                uiObject = Resources.Load<GameObject>("Prefabs/UI/Knife");
                heldObject = Resources.Load<GameObject>("Prefabs/Held/M9_Knife");
                localPosition = new Vector3(1f ,-0.35f ,1.15f);
                localRotation = new Vector3(180f, 0, 0);
                damage = 50f;
                name = "Knife";
            break;
            case itemIDs.healthPotion:
                cellSize = new Vector2Int(1, 1);
                uiObject = Resources.Load<GameObject>("Prefabs/UI/HealthPotion");
                heldObject = Resources.Load<GameObject>("Prefabs/Held/HealthPotion");
                localPosition = new Vector3(1f ,-0.35f ,1.15f);
                localRotation = new Vector3(0, 0, 0);
                name = "Health Potion";
            break;
            case itemIDs.pistol:
                cellSize = new Vector2Int(1, 1);
                uiObject = Resources.Load<GameObject>("Prefabs/UI/Pistol");
                heldObject = Resources.Load<GameObject>("Prefabs/Held/Pistol");
                localPosition = new Vector3(0.91f, -0.3f, 1.13f);
                localRotation = new Vector3(0, -82.57f, 0);
                damage = 25f;
                name = "Pistol";
            break;
        }
    }
    public InventoryItem(itemIDs itemID)
    {
        type = itemID;

        InitializeOnType();

        uiObject = Object.Instantiate(uiObject);

        rectTransform = uiObject.GetComponent<RectTransform>();
    }
}
