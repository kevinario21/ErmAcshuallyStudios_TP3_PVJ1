using UnityEngine;

// Ítem recolectable del mapa. Su Collider debe tener Is Trigger activado.
// Al tocarlo, avisa al GameManager, que suma los puntos y notifica al inventario y al HUD.
public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData data;
    [SerializeField] private float spinSpeed = 90f;

    private void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null) return;
        if (data == null || GameManager.Instance == null) return;

        GameManager.Instance.CollectItem(data);
        Destroy(gameObject);
    }
}
