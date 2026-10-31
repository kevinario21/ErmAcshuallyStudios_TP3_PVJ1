using UnityEngine;

// Salud del jugador. La vida máxima sale del ScriptableObject PlayerStats.
// No decide qué pasa al morir: se lo notifica al GameManager.
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    private int maxHealth;
    private int currentHealth;
    private float lastHitTime;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    private void Start()
    {
        if (stats == null)
        {
            Debug.LogError("PlayerHealth: falta asignar el PlayerStats en " + name);
            enabled = false;
            return;
        }

        maxHealth = stats.MaxHealth;
        lastHitTime = -999f;

        // Conserva la vida del nivel anterior (persistencia entre escenas)
        currentHealth = maxHealth;
        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.GetSessionHealth(maxHealth);
            GameManager.Instance.ReportHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || !enabled) return;
        if (Time.time - lastHitTime < stats.InvulnerableTime) return;
        lastHitTime = Time.time;

        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReportDamage(amount, currentHealth, maxHealth);
            if (currentHealth == 0) GameManager.Instance.ReportPlayerDied();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead || !enabled) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        if (GameManager.Instance != null)
            GameManager.Instance.ReportHealth(currentHealth, maxHealth);
    }
}
