using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaSpawner : MonoBehaviour
{
    [Header("Verificare Nivel")]
    public HoleFeeding stomachScript;

    [Header("Puncte de Spawn")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("Ce aruncăm?")]
    public List<GameObject> prefabsToSpawn;

    [Header("Reguli Automatizare")]
    public int maxObjectsInScene = 3;
    public float spawnDelay = 1f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Așteptăm exact delay-ul setat de tine
            yield return new WaitForSeconds(spawnDelay);

            // Verificăm dacă a început lupta
            if (stomachScript == null || !stomachScript.isBossLevel) continue;

            // Funcția minune: găsește absolut toate obiectele active cu tag-ul "Food"
            GameObject[] allFoodObjects = GameObject.FindGameObjectsWithTag("Food");

            // Dacă sunt mai puține decât limita ta...
            if (allFoodObjects.Length < maxObjectsInScene)
            {
                // ...mai aruncăm din tavan!
                SpawnObject(leftSpawnPoint);
                SpawnObject(rightSpawnPoint);
            }
        }
    }

    private void SpawnObject(Transform spawnPoint)
    {
        if (prefabsToSpawn.Count == 0 || spawnPoint == null) return;

        int randomIndex = Random.Range(0, prefabsToSpawn.Count);
        GameObject prefabToDrop = prefabsToSpawn[randomIndex];

        // Creăm obiectul nou
        Instantiate(prefabToDrop, spawnPoint.position, Random.rotation);
    }
}