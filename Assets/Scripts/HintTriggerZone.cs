using UnityEngine;

public class HintTriggerZone : MonoBehaviour
{
    [Header("Datos de la pista")]
    [SerializeField] private HintData hintData; // Datos de la pista a mostrar, ScriptableObject

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null) return;

        if (hintData != null && HintUIManager.Instance != null)
        {
            HintUIManager.Instance.ShowHint(hintData.hintText);
        }
    } 

    //Ocultar la pista cuando el jugador no este cerca del objeto
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null) return;
        if (HintUIManager.Instance != null)
        {
            HintUIManager.Instance.HideHint();
        }
    }
}
