using TMPro;
using UnityEngine;

public class HintUIManager : MonoBehaviour
{
    public static HintUIManager Instance { get; private set; }

    [Header("Referencias de UI")]
    [SerializeField] private GameObject hintPanel; // Panel de la pista
    [SerializeField] private TextMeshProUGUI hintText; // Texto de la pista

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideHint();
    }

    public void ShowHint(string hintData)
    {
        if (hintText != null) hintText.text = hintData;
        if (hintPanel != null) hintPanel.SetActive(true);
    }

    public void HideHint()
    {
        if (hintPanel != null) hintPanel.SetActive(false);
    }
}
