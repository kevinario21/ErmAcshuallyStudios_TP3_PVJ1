using UnityEngine;

public class LevelExitTrigger : MonoBehaviour
{
    private bool levelCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !levelCompleted)
        {
            levelCompleted = true;

            Debug.Log("¡Nivel completado!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteLevel();
            }
            else
            {
                Debug.LogWarning("No se encontró el GameManager.");
            }
        }
    }
}
