using UnityEngine;
using UnityEngine.AI;

// Dron centinela: patrulla un área y persigue al jugador al detectarlo.
// Todos sus valores salen del ScriptableObject EnemyData.
// Necesita un NavMeshAgent y estar sobre el NavMesh horneado.
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private enum State { Patrol, Chase, Attack }

    [Header("Datos del enemigo")]
    [SerializeField] private EnemyData data;

    [Header("Puntos de patrulla")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("Indicador de alerta")]
    [SerializeField] private GameObject alertIndicator;

    private NavMeshAgent agent;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private Transform player;
    private Renderer enemyRenderer;
    private Color originalColor;
    private State state;
    private int currentPoint;
    //private bool warnedNoNavMesh;
    private float waitTimer;
    private float repathTimer;
    private float nextAttackTime;
    private float eyeHeight = 1f;
    private float repathInterval= 0.2f; 
    private Vector3 homePosition;

    private void Start()
    {
        if (data == null)
        {
            Debug.LogError("EnemyAi: falta asignar EnemyData en " + name);
            enabled = false;
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponent<Renderer>();

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
        if (alertIndicator != null)
        {
            alertIndicator.SetActive(false);
        }
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;

            if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
                agent.Warp(hit.position);
        }

        //Busca al jugador en la escena
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            player = playerMovement.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        homePosition = transform.position;
        agent.stoppingDistance = Mathf.Min(0.8f, data.AttackRange);
        StartPatrol();
    }

    private void Update()
    {
        if (player == null || !agent.isOnNavMesh) return;

        switch (state)
        {
            case State.Patrol:
                UpdatePatrol();
                break;

            case State.Chase:
                UpdateChase();
                break;

            case State.Attack:
                UpdateAttack();
                break;
        }
    }

    // PATRULLA
    private void StartPatrol()
    {
        state = State.Patrol;
        agent.isStopped = false;
        agent.speed = data.PatrolSpeed;
        SetEnemyColor(originalColor);
        if (alertIndicator != null)
        {
            alertIndicator.SetActive(false);
        }
        currentPoint = -1;
        waitTimer = 0f;
        GoToNextPoint();
    }

    private void UpdatePatrol()
    {
        if (CanDetectPlayer())
        {
            StartChase();
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= data.WaitAtPoint)
            {
                waitTimer = 0f;
                GoToNextPoint();
            }
        }
    }

    // DETECCIÓN POR VISIÓN O PROXIMIDAD
    private bool CanDetectPlayer()
    {
        if (playerMovement.Runtime.Invisible)
            return false;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= data.ProximityRadius)
            return true;

        return CanSeePlayer();
    }

    private void GoToNextPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        int nextPoint = Random.Range(0, patrolPoints.Length);

        while (patrolPoints.Length > 1 && nextPoint == currentPoint)
        {
            nextPoint = Random.Range(0, patrolPoints.Length);
        }

        currentPoint = nextPoint;

        if (patrolPoints[currentPoint] == null)
            return;

        agent.SetDestination(patrolPoints[currentPoint].position);
    }

    // PERSECUCIÓN
    private void StartChase()
    {
        state = State.Chase;
        agent.isStopped = false;
        agent.speed = data.ChaseSpeed;
        SetEnemyColor(Color.yellow);
        if (alertIndicator != null)
        {
            alertIndicator.SetActive(true);
        }
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
        if (distance <= data.AttackRange)
        {
            StartAttack();
            return;
        }

        repathTimer += Time.deltaTime;
        if (repathTimer >= repathInterval)
        {
            repathTimer = 0f;
            agent.SetDestination(player.position);
        }
    }

    // ATAQUE
    private void StartAttack()
    {
        state = State.Attack;
        agent.isStopped = true;
        SetEnemyColor(Color.red);
        nextAttackTime = 0f;
    }
    private void UpdateAttack()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > data.LoseRadius || playerMovement.Runtime.Invisible)
        {
            StartPatrol();
            return;
        }

        if (distance > data.AttackRange)
        {
            StartChase();
            return;
        }
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (Time.time >= nextAttackTime)
        {
            if (playerHealth != null)
                playerHealth.TakeDamage(data.ContactDamage);
            else
                Debug.LogWarning("EnemyAI: el jugador no tiene PlayerHealth.");

            nextAttackTime = Time.time + data.AttackCooldown;
        }
    }

    // DETECCIÓN
    private bool CanSeePlayer()
    {
        if (playerMovement.Runtime.Invisible) return false;

        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 toPlayer = (player.position + Vector3.up * 0.5f) - eye;

        if (toPlayer.magnitude > data.DetectionRadius) return false;
        if (Vector3.Angle(transform.forward, toPlayer) > data.FieldOfView * 0.5f) return false;

        RaycastHit hitR;

        if (Physics.Raycast(eye, toPlayer.normalized, out hitR, data.DetectionRadius, ~0, QueryTriggerInteraction.Ignore))
            return hitR.transform==player || hitR.transform.IsChildOf(player);

        return false;
    }

    // CAMBIAR COLOR SEGÚN EL ESTADO
    private void SetEnemyColor(Color color)
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = color;
        }
    }

    //VISUALIZACION DE ZONAS Y CAMPO VISUAL
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        // Zona de detección por visión.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.DetectionRadius);
        // Zona de pérdida del jugador.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.LoseRadius);
        // Zona de ataque.
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, data.AttackRange);
        // Zona de detección por proximidad.
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, data.ProximityRadius);

        // Campo visual del enemigo.
        float halfAngle = data.FieldOfView * 0.5f;

        Vector3 leftDirection = Quaternion.Euler(0, -halfAngle, 0) * transform.forward;
        Vector3 rightDirection = Quaternion.Euler(0, halfAngle, 0) * transform.forward;

        Vector3 leftPoint = transform.position + leftDirection * data.DetectionRadius;
        Vector3 rightPoint = transform.position + rightDirection * data.DetectionRadius;

        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, leftPoint);
        Gizmos.DrawLine(transform.position, rightPoint);
    }
}