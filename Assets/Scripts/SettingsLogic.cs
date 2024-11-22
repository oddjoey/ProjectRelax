using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsLogic : MonoBehaviour
{
    GameLogic game;
    [SerializeField] public float sensitivity = 20.0f;
    [SerializeField] public float volume = 100.0f;
    [Serializable] public class ItemData
    {
        public int x, y;
        public int type;
    }
    [Serializable] public class CarData
    {
        public float HP;
        public float MaxRPM;
        public float BrakePower;
        public float SusDrop;
        public float SusCamber;
        public float SusOffset;
        public float TransFinalDriveRatio;
        public float ECUMaxSpeed;
    }
    [Serializable] public class GameData
    {
        public float sensitivity;
        public float volume;
        public int quest;
        public List<ItemData> inventory;
        public List<ItemData> hotbar;
        public CarData carData;
        public GameData()
        {
            inventory = new List<ItemData>();
            hotbar = new List<ItemData>();
            carData = new CarData();
        }
    }
    private string dataPath;
    public void SavePlayerData()
    {
        GameData data = new GameData();

        // Save car data
        var carObj = GameObject.Find("Car");
        if (carObj != null)
        {   
            var car = carObj.GetComponent<CarLogic>();
            data.carData.HP = car.drivetrain.engineTorque / 10;
            data.carData.MaxRPM = car.drivetrain.maximumEngineRPM;
            data.carData.BrakePower = car.drivetrain.brakeTorque / 100;
            data.carData.SusDrop = car.wheels.suspensionDrop;
            data.carData.SusCamber= car.wheels.susCamber * -1;
            data.carData.SusOffset = car.wheels.susOffset;
            data.carData.TransFinalDriveRatio = car.drivetrain.finalDriveRatio;
            data.carData.ECUMaxSpeed = car.drivetrain.maximumSpeed;
        }
        
        // Save game settings
        data.sensitivity = sensitivity;
        data.volume = volume;
        
        // Save game data
        data.quest = game.quests.questNumber;

        // Save inventory items
        var inventoryItems = game.inventory.GetItemsInCells(game.inventory.inventoryGrid, 0, 0, game.inventory.inventoryGrid.size.x, game.inventory.inventoryGrid.size.y);
        foreach (var item in inventoryItems)
        {
            if (item == null)
                continue;

            if (item.uiCellPosition != item.uiCellOrigin)
                continue;

            ItemData itemData = new ItemData();
            itemData.x = item.uiCellOrigin.x;
            itemData.y = item.uiCellOrigin.y;
            itemData.type = (int)item.type;
            data.inventory.Add(itemData);
        }

        // Save hotbar items
        var hotbarItems = game.inventory.GetItemsInCells(game.inventory.hotbarGrid, 0, 0, game.inventory.hotbarGrid.size.x, game.inventory.hotbarGrid.size.y);
        foreach (var item in hotbarItems)
        {
            if (item == null)
                continue;

            if (item.uiCellPosition != item.uiCellOrigin)
                continue;

            ItemData itemData = new ItemData();
            itemData.x = item.uiCellOrigin.x;
            itemData.y = item.uiCellOrigin.y;
            itemData.type = (int)item.type;
            data.hotbar.Add(itemData);
        }

        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(dataPath, jsonData);
    }
    public void LoadPlayerData()
    {
        if (!DataExists())
            return;
            
        GameData data = JsonUtility.FromJson<GameData>(File.ReadAllText(dataPath));

        var carObj = GameObject.Find("Car");
        if (game.localPlayer.inGameWorld)
        {
            var car = carObj.GetComponent<CarLogic>();
            car.drivetrain.engineTorque = data.carData.HP * 10;
            car.drivetrain.maximumEngineRPM = data.carData.MaxRPM;
            car.drivetrain.brakeTorque = data.carData.BrakePower * 100;
            car.wheels.suspensionDrop = data.carData.SusDrop;
            car.wheels.susCamber = data.carData.SusCamber * -1;
            car.wheels.susOffset = data.carData.SusOffset;
            car.drivetrain.finalDriveRatio = data.carData.TransFinalDriveRatio;
            car.drivetrain.maximumSpeed = data.carData.ECUMaxSpeed;
        }

        sensitivity = data.sensitivity;
        volume = data.volume;
        game.quests.questNumber = data.quest;

        game.inventory.ClearPanel(ref game.inventory.inventoryGrid);
        game.inventory.ClearPanel(ref game.inventory.hotbarGrid);
        foreach (var itemData in data.inventory)
        {
            InventoryItem item = new InventoryItem((InventoryItem.itemIDs)itemData.type);
            game.inventory.AddItem(ref game.inventory.inventoryGrid, ref item, itemData.x, itemData.y);
        }
        foreach (var itemData in data.hotbar)
        {
            InventoryItem item = new InventoryItem((InventoryItem.itemIDs)itemData.type);
            game.inventory.AddItem(ref game.inventory.hotbarGrid, ref item, itemData.x, itemData.y);
        }
    }
    public bool DataExists()
    {
        return File.Exists(dataPath);
    }
    void Start()
    {
        game = GameLogic.instance;
        dataPath = Application.persistentDataPath + "/data.json";
    }
    void Update()
    {
        
    }
}
