using UnityEngine;

// Datos de un ítem. Create > Parkour > Item Data.
// Lo usan los ítems del mapa (ItemPickup) y el inventario (ícono, nombre y descripción).
[CreateAssetMenu(fileName = "ItemData", menuName = "Parkour/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string displayName = "Ítem";
    [SerializeField] private string itemTag = "Item";
    [SerializeField] [TextArea] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private int points = 10;
    [SerializeField] private GameObject prefab;

    public string DisplayName => displayName;
    public string ItemTag => itemTag;
    public string Description => description;
    public Sprite Icon => icon;
    public int Points => points;
    public GameObject Prefab => prefab;
}
