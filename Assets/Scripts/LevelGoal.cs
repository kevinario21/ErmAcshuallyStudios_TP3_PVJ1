using UnityEngine;

// Meta del nivel (Collider con Is Trigger). Al entrar el jugador, el GameManager
// pasa al siguiente nivel conservando los datos, o gana si era el último.
public class LevelGoal : MonoBehaviour
{
    private bool reached;

    private void OnTriggerEnter(Collider other)
    {
        if (reached) return;
        if (other.GetComponentInParent<PlayerMovement>() == null) return;
        if (GameManager.Instance == null) return;

        reached = true;
        GameManager.Instance.CompleteLevel();
    }
}
