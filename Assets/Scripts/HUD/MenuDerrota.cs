using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuMuerte : UIPanel
{
    [SerializeField] private Button btnReintentar;
    [SerializeField] private Button btnMenuPrincipal;

    public override void Mostrar()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pausar el juego al morir para que todo se detenga

        // Botón Reintentar: Reactiva el tiempo y recarga la escena actual
        btnReintentar.onClick.RemoveAllListeners();
        btnReintentar.onClick.AddListener(() => {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        // Botón Menú Principal: Reactiva el tiempo y vuelve al menú
        btnMenuPrincipal.onClick.RemoveAllListeners();
        btnMenuPrincipal.onClick.AddListener(() => {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MenuInicio"); // Cambia por el nombre exacto de tu escena de menú
        });
    }

    public override void Ocultar()
    {
        gameObject.SetActive(false);
    }
}