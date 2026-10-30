using UnityEngine;

// Patrón Strategy: cada estrategia define CÓMO se mueve el jugador.
public interface IMovementStrategy
{
    // input.x = lateral, input.y = adelante/atrás -> se mapea a X y Z
    void Move(Rigidbody rb, PlayerRuntime stats, Vector2 input);
}

// Movimiento directo: la velocidad cambia al instante.
public class SmoothMovement : IMovementStrategy
{
    public void Move(Rigidbody rb, PlayerRuntime stats, Vector2 input)
    {
        Vector3 target = new Vector3(input.x, 0f, input.y);
        if (target.sqrMagnitude > 1f) target.Normalize();
        target *= stats.Velocity;

        Vector3 current = rb.linearVelocity;
        rb.linearVelocity = new Vector3(target.x, current.y, target.z);
    }
}

// Movimiento con aceleración: acelera al presionar y frena al soltar.
public class AccelerateMovement : IMovementStrategy
{
    public void Move(Rigidbody rb, PlayerRuntime stats, Vector2 input)
    {
        Vector3 target = new Vector3(input.x, 0f, input.y);
        if (target.sqrMagnitude > 1f) target.Normalize();
        target *= stats.Velocity;

        Vector3 current = rb.linearVelocity;
        Vector3 horizontal = new Vector3(current.x, 0f, current.z);

        horizontal = Vector3.MoveTowards(horizontal, target, stats.Acceleration * Time.fixedDeltaTime);

        // Se conserva la velocidad vertical (gravedad / salto)
        rb.linearVelocity = new Vector3(horizontal.x, current.y, horizontal.z);
    }
}

// Valores del jugador que pueden cambiar durante el juego (zonas, power-ups, mejoras).
// Se inicializa desde el ScriptableObject PlayerStats, que nunca se modifica.
public class PlayerRuntime
{
    private readonly float baseVelocity;
    private float velocity;
    private float acceleration;
    private bool invisible;

    public PlayerRuntime(PlayerStats data)
    {
        baseVelocity = data.MoveSpeed;
        velocity = data.MoveSpeed;
        acceleration = data.Acceleration;
        invisible = false;
    }

    public float BaseVelocity => baseVelocity;
    public float Velocity { get => velocity; set => velocity = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public bool Invisible { get => invisible; set => invisible = value; }

    public void ResetVelocity()
    {
        velocity = baseVelocity;
    }
}
