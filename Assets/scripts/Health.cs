using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int max = 3;
    [SerializeField] float iFrames = 1f;
    public bool shielded;
    int current;
    float invulnerableUntil;

    public int Current => current;
    public int Max => max;

    void Awake() => current = max;

    public bool TakeDamage(int amount)
    {
        if (current <= 0 || Time.time < invulnerableUntil) return false;
        invulnerableUntil = Time.time + iFrames;
        if (shielded) { shielded = false; return false; }
        current -= amount;
        GameManager.Instance.UpdateUI();
        if (current <= 0) GameManager.Instance.PlayerDied();
        return true;
    }

    public void Restore(float protection)
    {
        current = max;
        invulnerableUntil = Time.time + protection;
        GameManager.Instance.UpdateUI();
    }
}
