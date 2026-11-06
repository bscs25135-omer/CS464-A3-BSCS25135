using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] int targetScore = 10;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject winPanel;

    [Header("Bonus Round")]
    [SerializeField] GameObject redCoinPrefab;
    [SerializeField] GameObject blueCoinPrefab;
    [SerializeField] Transform player;
    [SerializeField] TMP_Text bonusText;
    [SerializeField] int bonusCoinCount = 50;
    [SerializeField] float bonusMinRadius = 1.5f;
    [SerializeField] float bonusMaxRadius = 6f;
    [SerializeField] float arenaLimit = 9f;

    int score;
    int redTriggerScore;
    bool redSpawned;
    bool redOnField;
    int bonusCollected;
    int bonusRemaining;

    public bool NormalCoinsNeeded => score < targetScore;

    void Awake() => Instance = this;

    void Start()
    {
        redTriggerScore = Random.Range(1, targetScore + 1);
        Debug.Log($"Red coin will spawn when score reaches {redTriggerScore}");
        if (bonusText != null) bonusText.gameObject.SetActive(false);
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score = Mathf.Min(score + amount, targetScore);
        UpdateUI();
        if (!redSpawned && score >= redTriggerScore) SpawnRedCoin();
        CheckWin();
    }

    void SpawnRedCoin()
    {
        redSpawned = true;
        redOnField = true;

        Vector3 pos = Vector3.zero;
        for (int i = 0; i < 20; i++)
        {
            pos = new Vector3(Random.Range(-arenaLimit, arenaLimit), 0.5f,
                              Random.Range(-arenaLimit, arenaLimit));
            if (Vector3.Distance(pos, player.position) > 3f) break;
        }
        Instantiate(redCoinPrefab, pos, Quaternion.identity);
        Debug.Log("Red coin spawned");
    }

    public void CollectRedCoin(Vector3 center)
    {
        redOnField = false;
        bonusCollected = 0;
        bonusRemaining = bonusCoinCount;

        for (int i = 0; i < bonusCoinCount; i++)
        {
            float angle = Random.value * Mathf.PI * 2f;
            float radius = Mathf.Sqrt(Random.Range(
                bonusMinRadius * bonusMinRadius,
                bonusMaxRadius * bonusMaxRadius));

            float x = Mathf.Clamp(center.x + Mathf.Cos(angle) * radius, -arenaLimit, arenaLimit);
            float z = Mathf.Clamp(center.z + Mathf.Sin(angle) * radius, -arenaLimit, arenaLimit);
            Instantiate(blueCoinPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
        }

        bonusText.gameObject.SetActive(true);
        UpdateBonusUI();
    }

    public void CollectBlueCoin()
    {
        bonusCollected++;
        bonusRemaining--;
        UpdateBonusUI();
        CheckWin();
    }

    void CheckWin()
    {
        if (score >= targetScore && !redOnField && bonusRemaining <= 0) Win();
    }

    void UpdateUI() => scoreText.text = $"Score: {score} / {targetScore}";

    void UpdateBonusUI() =>
        bonusText.text = $"BonusCoins: {bonusCollected}/{bonusCoinCount}";

    void Win() { winPanel.SetActive(true); Time.timeScale = 0f; }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}