# CarrotRush — Assignment 11 Decision Log

2026-11-18 — Used AI assistance to organize game balance into ScriptableObject assets for enemies, the player, carrots, and bullets. Kept existing mechanics and tested changes in Unity.
2026-11-18 — Tried adding a visible Patroller lunge, but it disrupted the chase behavior. Reverted to the simpler warning-and-damage attack for reliability.
2026-11-18 — Created three LevelData assets and updated LevelFlow to spawn enemy prefabs and connect them to the player at runtime.
2026-11-18 — Level Select initially opened Meadow after selecting Level 2. Adjusted LevelFlow to use the selected level when reloading the scene.
2026-11-18 — Added JSON saving for scores and unlocked levels, plus default data for failed loads. Settings persistence and final phone restart testing still need verification.
