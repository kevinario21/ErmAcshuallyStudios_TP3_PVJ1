using UnityEngine;

// Datos de un tipo de enemigo. Create > Parkour > Enemy Data.
// SentinelAI lee todos sus valores de acá: no hay números en el script.
[CreateAssetMenu(fileName = "EnemyData", menuName = "Parkour/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identidad")]
    [SerializeField] private string displayName = "Dron centinela";
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private GameObject prefab;

    [Header("Combate")]
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int contactDamage = 10;
    [SerializeField] private int pointsReward = 0;

    [Header("Patrulla")]
    [SerializeField] private float patrolRadius = 12f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitAtPoint = 1.5f;

    [Header("Detección y persecución")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float loseRadius = 14f;
    [SerializeField] private float fieldOfView = 120f;
    [SerializeField] private float chaseSpeed = 4.5f;

    public string DisplayName => displayName;
    public string EnemyTag => enemyTag;
    public GameObject Prefab => prefab;
    public int MaxHealth => maxHealth;
    public int ContactDamage => contactDamage;
    public int PointsReward => pointsReward;
    public float PatrolRadius => patrolRadius;
    public float PatrolSpeed => patrolSpeed;
    public float WaitAtPoint => waitAtPoint;
    public float DetectionRadius => detectionRadius;
    public float LoseRadius => loseRadius;
    public float FieldOfView => fieldOfView;
    public float ChaseSpeed => chaseSpeed;
}
