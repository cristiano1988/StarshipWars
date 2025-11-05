using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Timing")]
    public float minDelay = 0.5f;
    public float maxDelay = 1.5f;

    [Header("Prefabs")]
    public GameObject enemyPrefab;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(wait);

            if (enemyPrefab == null) continue;

            float x = Random.Range(GameBounds.minX, GameBounds.maxX);
            float y = Random.Range(GameBounds.minY, GameBounds.maxY);

            int edge = Random.Range(0, 4);
            switch (edge)
            {
                case 0: y = GameBounds.maxY; break;
                case 1: y = GameBounds.minY; break;
                case 2: x = GameBounds.maxX; break;
                case 3: x = GameBounds.minX; break;
            }

            Vector3 spawnPos = new Vector3(x, y, 0);
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}