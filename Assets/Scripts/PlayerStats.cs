using UnityEngine;

// Datos base del jugador. Se crean desde Create > Parkour > Player Stats y se editan en el Inspector.
// Desde el código solo se LEEN: lo que cambia durante el juego vive en PlayerRuntime
// y en el GameManager, así el asset nunca se modifica al jugar.
[CreateAssetMenu(fileName = "PlayerStats", menuName = "Parkour/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Salud")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerableTime = 1f;
    [SerializeField] private int fallDamage = 10;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float turnSpeed = 720f;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Header("Inventario")]
    [SerializeField] private int inventoryCapacity = 12;

    public int MaxHealth => maxHealth;
    public float InvulnerableTime => invulnerableTime;
    public int FallDamage => fallDamage;
    public float MoveSpeed => moveSpeed;
    public float Acceleration => acceleration;
    public float TurnSpeed => turnSpeed;
    public float JumpForce => jumpForce;
    public float CoyoteTime => coyoteTime;
    public float JumpBufferTime => jumpBufferTime;
    public float GroundCheckDistance => groundCheckDistance;
    public int InventoryCapacity => inventoryCapacity;
}
