using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // ScriptableObject con los valores. Se arrastra en el Inspector.
    [SerializeField] private PlayerStats stats;

    private Rigidbody rb;
    private Collider col;
    private Transform cameraTransform;
    private PlayerRuntime runtime;
    private IMovementStrategy movementStrategy;
    private PlayerHealth health;

    private LayerMask groundMask;
    private Vector3 respawnPosition;
    private float fallLimit;

    private Vector2 moveInput;
    private float coyoteTimer;       // tiempo que queda para saltar tras dejar el suelo
    private float jumpBufferTimer;   // tiempo que queda de una pulsación de salto "guardada"

    // Valores que pueden cambiar en juego (barro, power-ups, mejoras de la tienda)
    public PlayerRuntime Runtime => runtime;

    private void Awake()
    {
        if (stats == null)
        {
            Debug.LogError("PlayerMovement: falta asignar el PlayerStats en " + name);
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        health = GetComponent<PlayerHealth>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        runtime = new PlayerRuntime(stats);
        SetMovementStrategy(new AccelerateMovement());

        // Solo cuentan como suelo los objetos de la capa "Ground"
        groundMask = LayerMask.GetMask("Ground");
        if (groundMask == 0)
        {
            Debug.LogWarning("PlayerMovement: no existe la capa Ground. Crearla y asignarla al suelo y las plataformas.");
            groundMask = ~0;
        }

        respawnPosition = transform.position;
        fallLimit = respawnPosition.y - 15f;
    }

    public void SetMovementStrategy(IMovementStrategy strategy)
    {
        movementStrategy = strategy;
    }

    // Los Checkpoint llaman a esto para cambiar el punto donde reaparece el jugador
    public void SetCheckpoint(Vector3 position)
    {
        respawnPosition = position;
        fallLimit = position.y - 15f;
    }

    public void Respawn()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = respawnPosition;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return; // juego en pausa

        // W A S D explícitos: las flechas quedan libres para la cámara
        float x = 0f;
        float z = 0f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.W)) z += 1f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;

        moveInput = CameraRelative(new Vector2(x, z));

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = stats.JumpBufferTime;

        RotateTowardsMovement();
    }

    private void FixedUpdate()
    {
        movementStrategy.Move(rb, runtime, moveInput);

        // Coyote time: se puede saltar unos instantes después de dejar el borde.
        // Jump buffer: si se aprieta salto justo antes de aterrizar, el salto se hace igual.
        bool grounded = IsGrounded();

    
        coyoteTimer = grounded ? stats.CoyoteTime : coyoteTimer - Time.fixedDeltaTime;
        jumpBufferTimer -= Time.fixedDeltaTime;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            Vector3 v = rb.linearVelocity;
            rb.linearVelocity = new Vector3(v.x, stats.JumpForce, v.z);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        if (transform.position.y < fallLimit)
        {
            if (health != null) health.TakeDamage(stats.FallDamage);
            Respawn();
        }
    }

    // W avanza hacia donde mira la cámara
    private Vector2 CameraRelative(Vector2 input)
    {
        if (cameraTransform == null) return input;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 direction = right * input.x + forward * input.y;
        return new Vector2(direction.x, direction.z);
    }

    private void RotateTowardsMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion target = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, stats.TurnSpeed * Time.deltaTime);
    }

    // Chequeo de suelo: Raycast desde los pies hacia abajo, filtrado por LayerMask
    private bool IsGrounded()
    {
        Bounds b = col.bounds;
        Vector3 feet = new Vector3(b.center.x, b.min.y + 0.05f, b.center.z);
        float distance = 0.05f + stats.GroundCheckDistance;

        Debug.DrawRay(feet, Vector3.down * distance, Color.green);

        return Physics.Raycast(feet, Vector3.down, distance, groundMask, QueryTriggerInteraction.Ignore);
    }
}
