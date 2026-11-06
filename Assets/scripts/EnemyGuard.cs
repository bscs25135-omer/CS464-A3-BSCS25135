using UnityEngine;

// Lecture 07, slides 20-23: Patrol -> Chase -> Return.
// Return needs no code of its own: EnemyPatrol is switched back on and walks
// to the waypoint it was heading for.
public class EnemyGuard : MonoBehaviour
{
    enum State { Patrol, Chase }

    [SerializeField] EnemyPatrol patrol;          // leave empty for wave guards
    [SerializeField] float chaseSpeed = 4f, turnSpeed = 360f;
    [SerializeField] float sightRange = 6f, sightAngle = 120f;
    [SerializeField] float memory = 1.5f;         // keeps chasing this long after losing sight
    [SerializeField] bool alwaysChase;            // wave guards always know where you are

    Transform player;
    State state = State.Patrol;
    float lastSeen = -999f;

    void Start() => player = GameObject.FindGameObjectWithTag("Player").transform;

    void Update()
    {
        if (alwaysChase || CanSee()) lastSeen = Time.time;
        state = Time.time - lastSeen < memory ? State.Chase : State.Patrol;

        if (patrol != null) patrol.enabled = state == State.Patrol;
        if (state == State.Chase) Chase();
    }

    bool CanSee()
    {
        Vector3 flat = player.position - transform.position;
        flat.y = 0f;
        if (flat.magnitude > sightRange) return false;
        if (Vector3.Angle(transform.forward, flat) > sightAngle * 0.5f) return false;

        Vector3 origin = new Vector3(transform.position.x, 0.5f, transform.position.z);
        Vector3 dir = player.position - origin;
        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, sightRange * 1.5f,
                            Physics.AllLayers, QueryTriggerInteraction.Ignore))
            return hit.transform == player;
        return false;
    }

    void Chase()
    {
        Vector3 target = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 toTarget = target - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed * Time.deltaTime);
        if (toTarget != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(toTarget), turnSpeed * Time.deltaTime);
    }
}
