using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuVictoria : UIPanel
{
    [SerializeField] private Button btnMenuPrincipal;

    public override void Mostrar()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; 

        btnMenuPrincipal.onClick.RemoveAllListeners();
        btnMenuPrincipal.onClick.AddListener(() => {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MenuInicio");
        });
    }

    public override void Ocultar()
    {
        gameObject.SetActive(false);
    }
}