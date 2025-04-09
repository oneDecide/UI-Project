using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab of the enemy to spawn
    public float spawnInterval = 2f; // Time interval between spawns

    public float spawnRadius = 5f; // Radius around the spawner to spawn enemies
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy()
    {
        // Instantiate the enemy prefab at the spawner's position and rotation
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        spawnPosition.y = transform.position.y; // Keep the y position the same as the spawner
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        // Set the enemy's parent to the spawner for organization (optional)
        enemy.transform.parent = transform;
        
    }
}
