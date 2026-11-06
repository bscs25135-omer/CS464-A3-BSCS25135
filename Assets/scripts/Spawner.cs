using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform player;
    [SerializeField] float interval = 1.5f;
    [SerializeField] int maxCoins = 5;
    [SerializeField] float area = 7f;

    void Start() => InvokeRepeating(nameof(SpawnCoin), 1f, interval);

    void SpawnCoin()
    {
        if (!GameManager.Instance.NormalCoinsNeeded) return;
        if (transform.childCount >= maxCoins) return;
        float x = Random.Range(-area, area);
        float z = Random.Range(-area, area);
        Vector3 pos = new Vector3(x, 0.5f, z);
        if (Vector3.Distance(pos, player.position) < 2f) return;
        Instantiate(coinPrefab, pos, Quaternion.identity, transform);
    }
}