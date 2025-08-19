using UnityEngine;

[System.Serializable]
public class SpawnPoint
{
    public int x;
    public int y;
    public string enemyType;
    public int count;
}

[System.Serializable]
public class BossPoint
{
    public int x;
    public int y;
    public string bossType;
}

[System.Serializable]
public class Wave
{
    public int waveId;
    public SpawnPoint[] spawnPoints;
}

[System.Serializable]
public class DropTables
{
    public float goldMine;
    public float bone;
}

[System.Serializable]
public class LevelData
{
    public int levelId;
    public Wave[] waves;
    public BossPoint bossPoint;
    public DropTables dropTables;
}

public class LevelLoader : MonoBehaviour
{
    public TextAsset jsonFile;

    public LevelData LoadLevelData()
    {
        if (jsonFile != null)
        {
            LevelData levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
            return levelData;
        }
        else
        {
            Debug.LogError("JSON file not assigned in the LevelLoader.");
            return null;
        }
    }
}
