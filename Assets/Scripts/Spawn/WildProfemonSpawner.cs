using UnityEngine;
using System.Collections.Generic;

public class WildProfemonSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<GameObject> wildPrefabs;
    public int maxAlive = 3;
    public float spawnInterval = 5f;

    [Header("Lifetime Settings")]
    public float lifetime = 20f; // Tiempo antes de desaparecer

    [Header("Spawn Area")]
    public Vector3 spawnAreaSize = new Vector3(10, 0, 10);

    private float timer;
    private List<GameObject> currentAlive = new List<GameObject>();

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            TrySpawn();
        }

        currentAlive.RemoveAll(item => item == null);
    }

    private void TrySpawn()
    {
        if (currentAlive.Count >= maxAlive)
            return;

        if (wildPrefabs.Count == 0)
            return;

        GameObject prefab = wildPrefabs[Random.Range(0, wildPrefabs.Count)];

        Vector3 randomPosition = transform.position +
            new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

        GameObject spawned = Instantiate(prefab, randomPosition, Quaternion.identity);

        currentAlive.Add(spawned);

        // 🔥 Iniciar autodestrucción
        StartCoroutine(DestroyAfterTime(spawned, lifetime));
    }

    private System.Collections.IEnumerator DestroyAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);

        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}