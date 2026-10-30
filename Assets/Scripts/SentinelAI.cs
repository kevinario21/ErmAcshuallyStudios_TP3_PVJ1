using UnityEngine;
using UnityEngine.AI;

// Dron centinela: patrulla un área y persigue al jugador al detectarlo.
// Todos sus valores salen del ScriptableObject EnemyData.
// Necesita un NavMeshAgent y estar sobre el NavMesh horneado.
[RequireComponent(typeof(NavMeshAgent))]
public class SentinelAI : MonoBehaviour
{
    private enum State { Patrol, Chase }

    [SerializeField] private EnemyData data;

    private NavMeshAgent agent;
    private PlayerMovement playerMovement;
    private Transform player;
    private PlayerHealth playerHealth;

    private State state;
    private Vector3 homePosition;
    private bool warnedNoNavMesh;
    private float waitTimer;
    private float repathTimer;

    private float eyeHeight;
    private float repathInterval;

    private void Start()
    {
        if (data == null)
        {
            Debug.LogError("SentinelAI: falta asignar el EnemyData en " + name);
            enabled = false;
            return;
        }

        eyeHeight = 1f;
        repathInterval = 0.2f;

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 1.2f;

        // Si el dron no quedó sobre el NavMesh, lo acomoda en el punto caminable más cercano
        if (!agent.isOnNavMesh)
        {
            NavMeshHit placeHit;
            if (NavMesh.SamplePosition(transform.position, out placeHit, 10f, NavMesh.AllAreas))
                agent.Warp(placeHit.position);
        }

        playerMovement = FindAnyObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            player = playerMovement.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        homePosition = transform.position;
        StartPatrol();
    }

    private void Update()
    {
        if (player == null) return;

        if (!agent.isOnNavMesh)
        {
            if (!warnedNoNavMesh)
            {
                warnedNoNavMesh = true;
                Debug.LogWarning("SentinelAI: " + name + " no está sobre el NavMesh. Hornear el NavMesh o moverlo.");
            }
            return;
        }

        if (state == State.Patrol) UpdatePatrol();
        else UpdateChase();
    }

    // ---------- PATRULLA ----------
    private void StartPatrol()
    {
        state = State.Patrol;
        agent.speed = data.PatrolSpeed;
        waitTimer = 0f;
        GoToRandomPoint();
    }

    private void UpdatePatrol()
    {
        if (CanSeePlayer())
        {
            StartChase();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= data.WaitAtPoint)
            {
                waitTimer = 0f;
                GoToRandomPoint();
            }
        }
    }

    private void GoToRandomPoint()
    {
        if (!agent.isOnNavMesh) return;

        Vector2 circle = Random.insideUnitCircle * data.PatrolRadius;
        Vector3 candidate = homePosition + new Vector3(circle.x, 0f, circle.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(candidate, out hit, 5f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    // ---------- PERSECUCIÓN ----------
    private void StartChase()
    {
        state = State.Chase;
        agent.speed = data.ChaseSpeed;
        repathTimer = 0f;
    }

    private void UpdateChase()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > data.LoseRadius || playerMovement.Runtime.Invisible)
        {
            StartPatrol();
            return;
        }

        repathTimer += Time.deltaTime;
        if (repathTimer >= repathInterval)
        {
            repathTimer = 0f;
            agent.SetDestination(player.position);
        }
    }

    // ---------- DETECCIÓN ----------
    private bool CanSeePlayer()
    {
        if (playerMovement.Runtime.Invisible) return false;

        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 toPlayer = (player.position + Vector3.up * 0.5f) - eye;

        if (toPlayer.magnitude > data.DetectionRadius) return false;
        if (Vector3.Angle(transform.forward, toPlayer) > data.FieldOfView * 0.5f) return false;

        RaycastHit hit;
        if (Physics.Raycast(eye, toPlayer.normalized, out hit, data.DetectionRadius, ~0, QueryTriggerInteraction.Ignore))
            return hit.transform.IsChildOf(player);

        return false;
    }

    // ---------- ATAQUE ----------
    private void OnCollisionStay(Collision collision)
    {
        if (data == null || playerHealth == null) return;

        if (collision.collider.transform.IsChildOf(player))
            playerHealth.TakeDamage(data.ContactDamage);
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.DetectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.LoseRadius);
    }
}
