using UnityEngine;

public enum PowerUpType { Speed, Shield }

public class PowerUp : MonoBehaviour
{
    [SerializeField] PowerUpType type = PowerUpType.Speed;
    [SerializeField] float multiplier = 1.6f;
    [SerializeField] float duration = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (type == PowerUpType.Speed)
            other.GetComponent<PlayerController>().BoostSpeed(multiplier, duration);
        else
            other.GetComponent<Health>().shielded = true;
        Destroy(gameObject);
    }
}
