using UnityEngine;

// Datos configurables de un enemigo. Create > Parkour > Enemy > Enemy Data.
// EnemyAI utiliza estos valores para controlar su comportamiento.
[CreateAssetMenu(fileName = "EnemyData", menuName = "Parkour/Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identidad")]
    [SerializeField] private string displayName = "Enemigo centinela";
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private GameObject prefab;

    [Header("Combate")]
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int contactDamage = 10;
    [SerializeField] private int pointsReward = 0;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Patrulla")]
    [SerializeField] private float patrolRadius = 12f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitAtPoint = 2.5f;

    [Header("Detección y persecución")]
    [SerializeField] private float detectionRadius = 8.5f;
    [SerializeField] private float proximityRadius = 2.5f;
    [SerializeField] private float loseRadius = 10f;
    [SerializeField] private float fieldOfView = 120f;
    [SerializeField] private float chaseSpeed = 4.0f;

    //Identidad
    public string DisplayName => displayName;
    public string EnemyTag => enemyTag;
    public GameObject Prefab => prefab;

    //Combate
    public int MaxHealth => maxHealth;
    public int ContactDamage => contactDamage;
    public int PointsReward => pointsReward;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;

    //Patrulla
    public float PatrolRadius => patrolRadius;
    public float PatrolSpeed => patrolSpeed;
    public float WaitAtPoint => waitAtPoint;


    //Detección
    public float DetectionRadius => detectionRadius;
    public float ProximityRadius => proximityRadius;
    public float LoseRadius => loseRadius;
    public float FieldOfView => fieldOfView;
    public float ChaseSpeed => chaseSpeed;
}
