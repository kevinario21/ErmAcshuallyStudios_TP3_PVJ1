using UnityEngine;

// Punto de control (Collider con Is Trigger). Al pasar, el jugador reaparece acá si se cae.
public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        player.SetCheckpoint(transform.position + Vector3.up);
    }
}
