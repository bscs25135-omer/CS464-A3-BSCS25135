using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveForce = 10f;
    [SerializeField] ForceMode mode = ForceMode.Force;
    Rigidbody rb;
    Transform cam;
    Vector3 input;
    float speedMultiplier = 1f;
    Coroutine boost;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Camera-relative: flatten the camera's forward and right onto the floor.
        Vector3 forward = cam.forward; forward.y = 0f;
        Vector3 right = cam.right; right.y = 0f;
        if (forward.sqrMagnitude < 0.01f) { forward = cam.up; forward.y = 0f; } // camera looking straight down
        forward.Normalize();
        right.Normalize();

        input = Vector3.ClampMagnitude(forward * v + right * h, 1f);
    }

    void FixedUpdate() => rb.AddForce(input * moveForce * speedMultiplier, mode);

    public void BoostSpeed(float multiplier, float seconds)
    {
        if (boost != null) StopCoroutine(boost);
        boost = StartCoroutine(Boost(multiplier, seconds));
    }

    IEnumerator Boost(float multiplier, float seconds)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(seconds);
        speedMultiplier = 1f;
        boost = null;
    }
}