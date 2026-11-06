using System.Collections;
using UnityEngine;

// Lecture 07, slides 67-70, plus a lifetime so a wave ends even though guards can't be killed.
public class WaveSpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int firstWave = 2, extraPerWave = 1;
    [SerializeField] float spawnGap = 0.8f, restBetweenWaves = 2f, enemyLifetime = 10f;
    int wave;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(restBetweenWaves);
        while (true)
        {
            wave++;
            int count = firstWave + (wave - 1) * extraPerWave;
            for (int i = 0; i < count; i++)
            {
                Transform at = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemy = Instantiate(enemyPrefab, at.position, at.rotation, transform);
                Destroy(enemy, enemyLifetime);
                yield return new WaitForSeconds(spawnGap);
            }
            yield return new WaitUntil(() => transform.childCount == 0);
            yield return new WaitForSeconds(restBetweenWaves);
        }
    }
}
