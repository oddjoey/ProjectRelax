# Project RelaX!

## Introduction

The game introduces you to your new car, which was given to you by your mother for your 16th birthday. The game's goal is to complete the quests shown on the right of the screen. The controls are simple: WASD for player movement, Mouse for the player looking, TAB for inventory, Space to jump, F to use/interact, and ESC for the menu. When in the car, W is the accelerator, S is the brakes, A & D are for steering, E is to start the engine, and Space is for the handbrake.
## Written Scripts

```
CarLogic.cs - Controls the car logic, such as lights, drivetrain, exhaust, and other components.
CarSuspensionLogic.cs - Controls car’s suspension like height, camber, and offset.
CutSceneLogic.cs - Logic for our cutscenes, like switching cameras, activating the next animations, etc.
CutSceneWalkingLogic.cs - Behavior for our animation, specifically setting the walking animation.
EscapeMenuLogic.cs - Our ESC menu logic sets our setting values, exits, resuming, etc.
GameLogic.cs - Holds other components that are used by other scripts.
GarageLogic.cs - Logic for modifying our car and leaving the garage.
HeldItemLogic.cs - Logic for items that are being held.
HotbarLogic.cs - Hotbar logic, scrolling, and which item is selected.
InputManager.cs - Handles our input to be used by other scripts.
InventoryItem.cs - Logic used for items in inventory, holds information about items such as size, name, prefabs, and position, which is the foundation for our inventory system.
InventoryLogic.cs - The inventory is a grid system similar to Escape From Tarkov or DayZ. It's one of my favorite inventory systems in a video game, and I’ve always wanted to try to do it myself with the help of the internet. This script handles all the logic for the inventory system and part of the Hotbar since it is also a mini inventory.
InventoryPanel.cs -  Logic for defining inventory panels currently used for the main inventory, and Hotbar could be expanded for chests/storage.
LocalPlayerLogic.cs - Movement and looking logic, held item logic, pickup logic, and entering/leaving vehicles. It also tracks where the local player is in terms of which scene.
MainMenuLogic.cs - Main menu logic, start, load, guide, settings, and quit.
MiniMapLogic.cs - Logic controls our minimap, sets the waypoint arrow, and determines camera orientation/position.
ModsMenuLogic.cs - The menu used to modify our car also sets values directly for the car.
PickupableItem.cs - Holds the type of item and is used by other scripts.
QuestLogic.cs - Logic for our quest system, which sets which quest is active and the logic for the quest themselves.
SettingsLogic.cs -  Sensitivity and volume are stored here, with the logic for saving and loading these settings, plus the player's inventory, car modifications, and which quest is current.
SoundLogic.cs - Controls playing sounds and if in a certain position.
TeleporterLogic.cs - Logic is used to set up scenes.
UILogic.cs - UI logic sets visibility for different UI aspects.
```

## Important Game Objects
```
Car: LocalPlayer can get into the car and drive around; it is used to navigate the city fast. The car also has lights, exhausts, wheels, the model, and the logic to run everything.
LocalPlayer: What the player controls and will interact with the world. 
Systems: Used to control different aspects of the game
Inputs: These are used to capture input for other scripts.
UI: Everything that has to do with the screen
SCC_Canvas: Speedometer and tachometer
HUD:
-Hotbar
-Inventory
-Crosshair
-Minimap
-Quests
-Escape Menu
-Main Menu
Settings: Stores settings of the game
Sounds: Sound storer and player
Teleporter: Used to travel to different scenes properly.
```

## Scenes
```
Mainmenu: Our main menu screen leads to other scenes; the background is our car with a city view.
IntroCutScene: The first cut scene, which is the intro, introduces you, your mom, and where you get your first car from.
City: The main gameplay scene of the game is the city, where there is a garage and many places to explore.
Garage: The place where our player can customize their car.
```

## Referenced Assets

Simple Keys - 3DIGITALIS - Asset As Is - https://assetstore.unity.com/packages/3d/props/tools/simple-keys-231162

Modular House Pack 1 - ICAROUS - Asset As Is - https://assetstore.unity.com/packages/3d/environments/urban/modular-house-pack-1-236466

Simple Garage - AIKODEX - Asset As Is - https://assetstore.unity.com/packages/3d/props/interior/simple-garage-197251

Simple Car Controller - BONECRACKER GAMES - Heavily modified - https://assetstore.unity.com/packages/tools/physics/simple-car-controller-258020

Demo City - VERSATILE STUDIO - Asset As Is - https://assetstore.unity.com/packages/3d/environments/urban/demo-city-by-versatile-studio-mobile-friendly-269772

Simple Retro Car - POLYELER - Asset As Is - https://assetstore.unity.com/packages/3d/vehicles/simple-retro-car-291522

Player Models/Animations - Mixamo - Slight Modification - https://www.mixamo.com/#/

## Guides/Tutorials

Game Dev Handbook: https://kb.heathen.group/

Learn Unity: https://learn.unity.com/

C# Guide: https://www.w3schools.com/cs/index.php

Docs and guides to work with the Unity ecosystem: https://docs.unity.com/

Unity 6 User Manual: https://docs.unity3d.com/6000.0/Documentation/Manual/UnityManual.html

HDRP (High Definition Render Pipeline): https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@17.0/manual/index.html

Fish-Net Networking: https://fish-networking.gitbook.io/docs

Steamworks.NET Overview: https://steamworks.github.io/ 

Steamworks API: https://partner.steamgames.com/doc/sdk/api

UI Toolkit (used for making menus): https://docs.unity3d.com/Manual/UIElements.html
