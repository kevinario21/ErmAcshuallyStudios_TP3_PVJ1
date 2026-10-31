using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Opcional E: panel de inventario (tecla Tab o I).
// Muestra los ítems que el GameManager guardó, en casilleros. Al hacer click en uno,
// muestra su ícono, nombre y descripción, que salen del ScriptableObject ItemData.
// Este script va en un objeto que SIEMPRE esté activo (por ejemplo el Canvas),
// y "panel" es el hijo que se muestra y se oculta.
public class InventoryUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Casilleros")]
    [SerializeField] private PlayerStats stats;          // de acá sale la capacidad del inventario
    [SerializeField] private Transform slotsParent;      // objeto con Grid Layout Group
    [SerializeField] private InventorySlotUI slotPrefab;

    [Header("Detalle del ítem seleccionado")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TMP_Text detailName;
    [SerializeField] private TMP_Text detailDescription;

    private readonly List<InventorySlotUI> slots = new List<InventorySlotUI>();

    private void Start()
    {
        int capacity = stats != null ? stats.InventoryCapacity : 12;

        for (int i = 0; i < capacity; i++)
        {
            InventorySlotUI slot = Instantiate(slotPrefab, slotsParent);
            slot.Setup(this);
            slots.Add(slot);
        }

        panel.SetActive(false);
        ShowDetail(null);

        if (GameManager.Instance != null)
            GameManager.Instance.OnItemCollected += HandleItemCollected;

        Refresh();   // por si ya había ítems del nivel anterior
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnItemCollected -= HandleItemCollected;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Tab) && !Input.GetKeyDown(KeyCode.I)) return;

        // Se puede abrir solo jugando; cerrar se puede siempre
        bool playing = GameManager.Instance != null && GameManager.Instance.State == GameState.Playing;
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
        else if (playing)
        {
            Refresh();
            panel.SetActive(true);
        }
    }

    private void HandleItemCollected(ItemData item)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (GameManager.Instance == null) return;

        IReadOnlyList<ItemData> items = GameManager.Instance.Items;
        for (int i = 0; i < slots.Count; i++)
            slots[i].SetItem(i < items.Count ? items[i] : null);
    }

    public void ShowDetail(ItemData item)
    {
        if (item == null)
        {
            detailIcon.enabled = false;
            detailName.text = "";
            detailDescription.text = "Seleccioná un ítem";
            return;
        }

        detailIcon.sprite = item.Icon;
        detailIcon.enabled = item.Icon != null;
        detailName.text = item.DisplayName;
        detailDescription.text = item.Description;
    }
}