using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum Objective { CollectCoins, ReachGoal, Survive, KeysThenExit }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Objective")]
    [SerializeField] Objective objective = Objective.CollectCoins;
    [SerializeField] int targetScore = 5;
    [SerializeField] float timeLimit = 0f;      // 0 = no timer. Survive: time to last. Others: time to finish.
    [SerializeField] GameObject goal;           // ReachGoal and KeysThenExit
    [SerializeField] bool finalLevel;

    [Header("Lives")]
    [SerializeField] int lives = 3;
    [SerializeField] float respawnDelay = 0.6f, spawnProtection = 1.5f;
    [SerializeField] Health player;
    [SerializeField] Transform respawnPoint;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText, livesText, timerText, bonusText;
    [SerializeField] GameObject winPanel, losePanel, nextButton;

    [Header("Bonus Round (Lecture 06)")]
    [SerializeField] bool bonusRound = true;
    [SerializeField] GameObject redCoinPrefab, blueCoinPrefab;
    [SerializeField] int bonusCoinCount = 20;
    [SerializeField] float bonusMinRadius = 1.5f, bonusMaxRadius = 6f, arenaLimit = 9f;

    int score, redTriggerScore, bonusCollected, bonusRemaining, bonusTotal;
    bool redSpawned, redOnField, ended;
    float remaining;

    public bool NormalCoinsNeeded => score < targetScore;

    void Awake() => Instance = this;

    void Start()
    {
        remaining = timeLimit;
        redTriggerScore = Random.Range(1, targetScore + 1);
        if (bonusText != null) bonusText.gameObject.SetActive(false);
        if (objective == Objective.KeysThenExit && goal != null) goal.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (ended || timeLimit <= 0f) return;
        remaining -= Time.deltaTime;
        if (remaining <= 0f)
        {
            remaining = 0f;
            UpdateUI();
            if (objective == Objective.Survive) Win(); else Lose();
            return;
        }
        UpdateUI();
    }

    // ---------- score and bonus round ----------
    public void AddScore(int amount)
    {
        if (ended) return;
        score = Mathf.Min(score + amount, targetScore);
        UpdateUI();
        if (bonusRound && !redSpawned && score >= redTriggerScore) SpawnRedCoin();
        CheckProgress();
    }

    bool FreeSpot(Vector3 pos) =>
        !Physics.CheckSphere(pos, 0.3f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

    void SpawnRedCoin()
    {
        redSpawned = true;
        redOnField = true;
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < 40; i++)
        {
            pos = new Vector3(Random.Range(-arenaLimit, arenaLimit), 0.5f,
                              Random.Range(-arenaLimit, arenaLimit));
            if (Vector3.Distance(pos, player.transform.position) > 3f && FreeSpot(pos)) break;
        }
        Instantiate(redCoinPrefab, pos, Quaternion.identity);
    }

    public void CollectRedCoin(Vector3 center)
    {
        redOnField = false;
        bonusCollected = 0;
        bonusRemaining = 0;
        for (int i = 0; i < bonusCoinCount; i++)
        {
            for (int tries = 0; tries < 10; tries++)
            {
                float angle = Random.value * Mathf.PI * 2f;
                float radius = Mathf.Sqrt(Random.Range(
                    bonusMinRadius * bonusMinRadius, bonusMaxRadius * bonusMaxRadius));
                float x = Mathf.Clamp(center.x + Mathf.Cos(angle) * radius, -arenaLimit, arenaLimit);
                float z = Mathf.Clamp(center.z + Mathf.Sin(angle) * radius, -arenaLimit, arenaLimit);
                Vector3 pos = new Vector3(x, 0.5f, z);
                if (!FreeSpot(pos)) continue;
                Instantiate(blueCoinPrefab, pos, Quaternion.identity);
                bonusRemaining++;
                break;
            }
        }
        bonusTotal = bonusRemaining;
        if (bonusText != null) bonusText.gameObject.SetActive(true);
        UpdateBonusUI();
        CheckProgress();
    }

    public void CollectBlueCoin()
    {
        bonusCollected++;
        bonusRemaining--;
        UpdateBonusUI();
        CheckProgress();
    }

    bool BonusDone => !bonusRound || (!redOnField && bonusRemaining <= 0);

    void CheckProgress()
    {
        if (ended || score < targetScore) return;
        if (objective == Objective.KeysThenExit)
        {
            if (goal != null) goal.SetActive(true);
            UpdateUI();
        }
        else if (objective == Objective.CollectCoins && BonusDone) Win();
    }

    public void ReachedGoal()
    {
        if (ended) return;
        if (objective == Objective.ReachGoal || objective == Objective.KeysThenExit) Win();
    }

    // ---------- UI ----------
    public void UpdateUI()
    {
        switch (objective)
        {
            case Objective.CollectCoins: scoreText.text = $"Coins: {score} / {targetScore}"; break;
            case Objective.ReachGoal:    scoreText.text = "Reach the goal!"; break;
            case Objective.Survive:      scoreText.text = "Survive!"; break;
            case Objective.KeysThenExit:
                scoreText.text = score < targetScore
                    ? $"Keys: {score} / {targetScore}" : "Vault open - reach the exit!";
                break;
        }
        livesText.text = $"Lives: {lives}   Health: {Mathf.Max(player.Current, 0)} / {player.Max}";
        if (timerText != null)
            timerText.text = timeLimit > 0f ? $"Time: {Mathf.CeilToInt(remaining)}" : "";
    }

    void UpdateBonusUI()
    {
        if (bonusText != null) bonusText.text = $"Bonus Coins: {bonusCollected}/{bonusTotal}";
    }

    // ---------- lives and the end of the game ----------
    public void PlayerDied()
    {
        if (ended) return;
        lives--;
        UpdateUI();
        if (lives <= 0) Lose(); else StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        Rigidbody body = player.GetComponent<Rigidbody>();
        body.position = respawnPoint.position;
        body.linearVelocity = Vector3.zero;
        player.Restore(spawnProtection);
    }

    void Win()
    {
        ended = true;
        winPanel.SetActive(true);
        if (nextButton != null) nextButton.SetActive(!finalLevel);
        Time.timeScale = 0f;
    }

    void Lose()
    {
        ended = true;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(next);
        else Restart();
    }
}
