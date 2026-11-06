using UnityEngine;

// Lecture 07, slide 28: a hazard on a steady rhythm.
// Put it on the same object as Hazard.cs, with a kinematic Rigidbody.
public class MovingHazard : MonoBehaviour
{
    [SerializeField] Vector3 direction = Vector3.right;
    [SerializeField] float speed = 1.5f, distance = 6f;
    Rigidbody rb;
    Vector3 start;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        start = rb.position;
    }

    void FixedUpdate()
    {
        float x = Mathf.PingPong(Time.time * speed, distance);
        rb.MovePosition(start + direction.normalized * x);
    }
}
