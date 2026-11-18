
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int version = 1;

    public int bestScore = 0;

    public int highestCompletedLevel = 0;

    public List<int> unlockedLevels = new List<int> { 1 };

    public float musicVolume = 0.8f;

    public float soundVolume = 1f;
}
