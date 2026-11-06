using System.Collections;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 6f;
    [SerializeField] float restTime = 0f, warnTime = 0.7f, strikeTime = 0.4f;
    [SerializeField] GameObject warning;

    IEnumerator Start()
    {
        if (restTime <= 0f) yield break;
        Collider zone = GetComponent<Collider>();
        while (true)
        {
            zone.enabled = false;
            yield return new WaitForSeconds(restTime);
            if (warning != null) warning.SetActive(true);
            yield return new WaitForSeconds(warnTime);
            if (warning != null) warning.SetActive(false);
            zone.enabled = true;
            yield return new WaitForSeconds(strikeTime);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out Health health) || !health.TakeDamage(damage)) return;
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;
        Vector3 away = body.position - transform.position;
        away.y = 0f;
        body.AddForce(away.normalized * knockback, ForceMode.Impulse);
    }
}
