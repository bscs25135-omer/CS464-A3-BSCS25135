using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveForce = 20f;
    [SerializeField] ForceMode mode = ForceMode.Force;
    Rigidbody rb;
    Vector3 input;

    void Awake() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        input = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1f);
    }

    void FixedUpdate() => rb.AddForce(input * moveForce, mode);
}