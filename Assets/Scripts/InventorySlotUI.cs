using UnityEngine;
using UnityEngine.UI;

// Un casillero del inventario. Va en el prefab del slot (un Button con un hijo Image para el ícono).
[RequireComponent(typeof(Button))]
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;   // la Image hija que muestra el ícono del ítem

    private ItemData item;
    private InventoryUI owner;

    public void Setup(InventoryUI inventory)
    {
        owner = inventory;
        GetComponent<Button>().onClick.AddListener(OnClick);
        SetItem(null);
    }

    // null = casillero vacío
    public void SetItem(ItemData data)
    {
        item = data;
        icon.sprite = data != null ? data.Icon : null;
        icon.enabled = data != null && data.Icon != null;
    }

    private void OnClick()
    {
        owner.ShowDetail(item);
    }
}