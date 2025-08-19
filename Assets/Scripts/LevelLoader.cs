using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Platformer.Mechanics
{
    public class LevelLoader : MonoBehaviour
    {
        public static LevelLoader Instance { get; private set; }

        [Header("Prefabs")]
        public GameObject catPrefab;
        public GameObject elephantPrefab;
        public GameObject giantElephantBossPrefab;

        [Header("Settings")]
        public float delayBetweenWaves = 2f;

        private LevelData currentLevelData;
        private int currentWaveIndex = 0;
        private List<GameObject> activeEnemies = new List<GameObject>();
        private Dictionary<string, GameObject> enemyPrefabs;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            enemyPrefabs = new Dictionary<string, GameObject>
            {
                { "Cat", catPrefab },
                { "Elephant", elephantPrefab }
            };
        }

        void Start()
        {
            LoadLevel("level1");
        }

        public void LoadLevel(string levelName)
        {
            string path = Path.Combine(Application.dataPath, "Data", levelName + ".json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                currentLevelData = JsonUtility.FromJson<LevelData>(json);
                Debug.Log("Level " + currentLevelData.levelId + " loaded.");
                StartCoroutine(SpawnWave(currentWaveIndex));
            }
            else
            {
                Debug.LogError("Could not find level file at: " + path);
            }
        }

        private IEnumerator SpawnWave(int waveIndex)
        {
            Wave wave = currentLevelData.waves[waveIndex];
            Debug.Log("Spawning wave " + wave.waveId);

            foreach (var spawnPoint in wave.spawnPoints)
            {
                if (enemyPrefabs.TryGetValue(spawnPoint.enemyType, out GameObject prefabToSpawn))
                {
                    for (int i = 0; i < spawnPoint.count; i++)
                    {
                        Vector3 position = new Vector3(spawnPoint.x, spawnPoint.y, 0);
                        GameObject enemy = Instantiate(prefabToSpawn, position, Quaternion.identity);

                        var enemyAI = enemy.GetComponent<EnemyAI>();
                        if (enemyAI != null)
                        {
                            enemyAI.loader = this;
                        }

                        activeEnemies.Add(enemy);
                    }
                }
                else
                {
                    Debug.LogWarning("Prefab for enemy type '" + spawnPoint.enemyType + "' not found.");
                }
            }

            yield return null;
        }

        private void SpawnBoss()
        {
            Debug.Log("Spawning the boss!");
            if (currentLevelData.bossPoint != null && giantElephantBossPrefab != null)
            {
                var spawnPoint = currentLevelData.bossPoint;
                Vector3 position = new Vector3(spawnPoint.x, spawnPoint.y, 0);
                GameObject boss = Instantiate(giantElephantBossPrefab, position, Quaternion.identity);

                var enemyAI = boss.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.loader = this;
                }
                activeEnemies.Add(boss);
            }
            else
            {
                Debug.LogWarning("Boss spawn point or prefab not set!");
            }
        }

        private IEnumerator ProceedToNextWave()
        {
            Debug.Log("Waiting " + delayBetweenWaves + " seconds before next wave...");
            yield return new WaitForSeconds(delayBetweenWaves);

            if (currentWaveIndex >= 5 || currentWaveIndex >= currentLevelData.waves.Count)
            {
                SpawnBoss();
            }
            else
            {
                StartCoroutine(SpawnWave(currentWaveIndex));
            }
        }

        public void EnemyDied(GameObject enemy)
        {
            activeEnemies.Remove(enemy);
            Debug.Log("An enemy died. Remaining: " + activeEnemies.Count);

            if (activeEnemies.Count == 0)
            {
                currentWaveIndex++;
                if (currentWaveIndex >= currentLevelData.waves.Count && currentLevelData.bossPoint == null) {
                    Debug.Log("All waves and bosses defeated! Level complete!");
                } else {
                    Debug.Log("Wave " + (currentWaveIndex-1) + " cleared!");
                    StartCoroutine(ProceedToNextWave());
                }
            }
        }
    }
}
