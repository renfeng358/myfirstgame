using System;
using System.Collections.Generic;

[System.Serializable]
public class SpawnPoint
{
    public float x;
    public float y;
    public string enemyType;
    public int count;
    public string bossType; // Can be null if it's not a boss point
}

[System.Serializable]
public class Wave
{
    public int waveId;
    public List<SpawnPoint> spawnPoints;
}

[System.Serializable]
public class LevelData
{
    public int levelId;
    public List<Wave> waves;
    public SpawnPoint bossPoint;
}
