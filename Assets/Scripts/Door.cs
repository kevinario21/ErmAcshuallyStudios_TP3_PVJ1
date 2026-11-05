using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform Pass; // la cerradura (opcional, si querés mostrar el item)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickItem pickItem = other.GetComponent<PickItem>();

            if (pickItem != null)
            {
                GameObject item = pickItem.DropItem();

                if (item != null)
                {
                    ItemInZone(item);
                    OpenDoor(); // ← ahora la puerta se desactiva
                    Debug.Log("Completaste el nivel");
                }
                else
                {
                    Debug.Log("Derrota o llegaste a la meta sin el item");
                }
            }
        }
    }

    private void ItemInZone(GameObject item)
    {
        // Opcional: mostrás el item en la cerradura
        if (Pass != null)
        {
            item.transform.SetParent(Pass);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }

        Collider col = item.GetComponent<Collider>();
        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (col != null) col.enabled = false;
        if (rb != null) rb.isKinematic = true;
    }

    private void OpenDoor()
    {
        // Desactiva la puerta para que el jugador pueda pasar
        gameObject.SetActive(false);
    }
}
