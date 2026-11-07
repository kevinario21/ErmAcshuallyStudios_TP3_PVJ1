using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Estados posibles del juego
public enum GameState { Start, Playing, Paused, Victory, GameOver }

// Controla el flujo del juego y guarda los datos de la partida que tienen que
// sobrevivir al cambio de escena (puntaje, vida e ítems).
// Se pone como prefab en CADA escena: si ya existe uno (porque viene de otra escena),
// el nuevo se destruye solo. Así se puede dar Play desde cualquier escena.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ---------- Datos de la partida (persisten entre escenas) ----------
    public GameState State { get; private set; }
    public int Score { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public IReadOnlyList<ItemData> Items => items;

    // ---------- Eventos para la UI y otros sistemas ----------
    public event Action<GameState> OnStateChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int, int> OnHealthChanged;   // vida actual, vida máxima
    public event Action<int> OnPlayerDamaged;        // cantidad de daño recibido
    public event Action<ItemData> OnItemCollected;

    private List<ItemData> items;
    private bool sessionStarted;
    private bool stateInitialized;
    private Scene lastScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        items = new List<ItemData>();
        lastScene = default;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        if (Instance != this) return;
        ApplySceneState(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        if (Instance != this) return;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        Instance = null;
    }

    private void Update()
    {
        if (Instance != this) return;

        bool pausePressed = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
        if (pausePressed && (State == GameState.Playing || State == GameState.Paused))
            TogglePause();
    }

    // ---------- ESTADO SEGÚN LA ESCENA ----------
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySceneState(scene);
    }

    private void ApplySceneState(Scene scene)
    {
        if (scene == lastScene) return;
        lastScene = scene;

        string sceneName = scene.name;
        if (sceneName == SceneNames.MainMenu)
        {
            SetState(GameState.Start);
        }
        else if (sceneName == SceneNames.Victory)
        {
            SetState(GameState.Victory);
        }
        else if (sceneName == SceneNames.GameOver)
        {
            SetState(GameState.GameOver);
        }
        else
        {
            // Escena de nivel: si no hay partida iniciada (Play directo), la crea
            if (!sessionStarted) BeginSession();
            SetState(GameState.Playing);
        }
    }

    private void SetState(GameState newState)
    {
        // Solo se congela el tiempo en pausa
        Time.timeScale = (newState == GameState.Paused) ? 0f : 1f;

        if (stateInitialized && State == newState) return;
        stateInitialized = true;
        State = newState;

        Debug.Log("[GameManager] Estado: " + State + " | Vida: " + CurrentHealth + "/" + MaxHealth +
                  " | Puntaje: " + Score);
        OnStateChanged?.Invoke(State);
    }

    private void BeginSession()
    {
        Score = 0;
        CurrentHealth = 0;   // 0 = todavía no se definió: el jugador arranca con la vida máxima
        MaxHealth = 0;
        items.Clear();
        sessionStarted = true;
        OnScoreChanged?.Invoke(Score);
    }

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    // ================== API PARA LOS MENÚS ==================
    public void StartNewGame()
    {
        BeginSession();
        LoadScene(SceneNames.Levels[0]);
    }

    public void TogglePause()
    {
        if (State == GameState.Playing) SetState(GameState.Paused);
        else if (State == GameState.Paused) SetState(GameState.Playing);
    }

    public void ResumeGame()
    {
        if (State == GameState.Paused) SetState(GameState.Playing);
    }

    public void LoadMainMenu()
    {
        sessionStarted = false;
        LoadScene(SceneNames.MainMenu);
    }

    public void QuitGame()
    {
        Debug.Log("[GameManager] Saliendo del juego");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ================== API PARA LOS NIVELES ==================
    // Pasa al siguiente nivel conservando puntaje, vida e ítems; si era el último, gana.
    public void CompleteLevel()
    {
        if (State != GameState.Playing) return;

        string current = SceneManager.GetActiveScene().name;
        int index = Array.IndexOf(SceneNames.Levels, current);

        if (index >= 0 && index + 1 < SceneNames.Levels.Length)
        {
            Debug.Log("[GameManager] Nivel completado: " + current + " -> " + SceneNames.Levels[index + 1]);
            LoadScene(SceneNames.Levels[index + 1]);
        }
        else
        {
            Win();
        }
    }

    public void Win()
    {
        if (State != GameState.Playing) return;
        SetState(GameState.Victory);
        LoadScene(SceneNames.Victory);
    }

    public void Lose()
    {
        if (State != GameState.Playing) return;
        SetState(GameState.GameOver);
        LoadScene(SceneNames.GameOver);
    }

    // ================== EVENTOS QUE REPORTAN JUGADOR, ENEMIGOS E ÍTEMS ==================
    public void AddScore(int points)
    {
        if (State != GameState.Playing) return;

        Score += points;
        Debug.Log("[GameManager] Puntaje: " + Score + " (+" + points + ")");
        OnScoreChanged?.Invoke(Score);
    }

    public void CollectItem(ItemData item)
    {
        if (State != GameState.Playing || item == null) return;

        items.Add(item);
        Debug.Log("[GameManager] Ítem recolectado: " + item.DisplayName);
        OnItemCollected?.Invoke(item);
        AddScore(item.Points);
    }

    // El jugador pregunta con cuánta vida arranca el nivel (conserva la del nivel anterior)
    public int GetSessionHealth(int maxHealth)
    {
        return CurrentHealth > 0 ? Mathf.Min(CurrentHealth, maxHealth) : maxHealth;
    }

    public void ReportHealth(int current, int max)
    {
        CurrentHealth = current;
        MaxHealth = max;
        OnHealthChanged?.Invoke(current, max);
    }

    public void ReportDamage(int amount, int current, int max)
    {
        Debug.Log("[GameManager] Daño recibido: " + amount + " | Vida: " + current + "/" + max);
        OnPlayerDamaged?.Invoke(amount);
        ReportHealth(current, max);
    }

    public void ReportPlayerDied()
    {
        Debug.Log("[GameManager] El jugador murió");
        Lose();
    }
}