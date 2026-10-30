using System.Collections.Generic;
using UnityEngine;

// Plataforma que va y viene entre su posición inicial y (inicial + moveOffset).
// Lleva consigo al jugador mientras está parado encima.
// Necesita Collider (sin Is Trigger) y la capa "Ground" para que el salto funcione sobre ella.
[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 moveOffset = new Vector3(6f, 0f, 0f);   // hasta dónde viaja
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 0.5f;                          // pausa en cada extremo

    private Rigidbody rb;
    private Collider platformCollider;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool goingToEnd;
    private float waitTimer;

    // Quién está parado encima en este momento
    private readonly HashSet<Rigidbody> riders = new HashSet<Rigidbody>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;   // se mueve por código, no por física
        platformCollider = GetComponent<Collider>();

        startPosition = transform.position;
        endPosition = startPosition + moveOffset;
        goingToEnd = true;
    }

    private void FixedUpdate()
    {
        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            riders.Clear();
            return;
        }

        Vector3 target = goingToEnd ? endPosition : startPosition;
        Vector3 next = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        Vector3 delta = next - rb.position;

        rb.MovePosition(next);

        // El jugador se mueve junto con la plataforma
        foreach (Rigidbody rider in riders)
            if (rider != null) rider.position += delta;
        riders.Clear();   // OnCollisionStay lo vuelve a llenar en el próximo paso de física

        if (next == target)
        {
            goingToEnd = !goingToEnd;
            waitTimer = waitTime;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody other = collision.rigidbody;
        if (other == null || collision.collider.GetComponentInParent<PlayerMovement>() == null) return;

        // Solo si está parado ARRIBA de la plataforma (no chocando de costado)
        if (collision.collider.bounds.min.y >= platformCollider.bounds.max.y - 0.2f)
            riders.Add(other);
    }

    // Línea de recorrido visible al seleccionar la plataforma en la ventana Scene
    private void OnDrawGizmosSelected()
    {
        Vector3 from = Application.isPlaying ? startPosition : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(from, from + moveOffset);
        Gizmos.DrawWireCube(from + moveOffset, transform.localScale);
    }
}