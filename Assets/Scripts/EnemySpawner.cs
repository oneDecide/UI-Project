using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab of the enemy to spawn
    public float spawnInterval = 2f; // Time interval between spawns

    public float spawnRadius = 5f; // Radius around the spawner to spawn enemies

    public GameObject[] enemySpawners;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);

        //get all enemyspawnerpucks in scene, tagged with "EnemySpawnLocation"
        enemySpawners = GameObject.FindGameObjectsWithTag("EnemySpawnLocation");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy()
    {

        //get the location of one of the pucks in the scene using enemySpawners array (get a random one)
        //use math random
        int randomIndex = Random.Range(0, enemySpawners.Length);
        GameObject randomEnemySpawner = enemySpawners[randomIndex];

    
        Vector2 spawnPosition = randomEnemySpawner.transform.position;
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        // Set the enemy's parent to the spawner for organization (optional)
        enemy.transform.parent = transform;
        
    }
}
