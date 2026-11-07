using UnityEngine;

public class GestorUI : MonoBehaviour
{
    [Header("Colocar los paneles en orden")]

    public UIPanel[] paneles;
    private UIPanel panelActual;

    void Start()
    {
        OcultarPaneles();
        MostrarPaneles(0);
    }

    public void MostrarPaneles(int indice)
    {
        if (indice < 0 || indice >= paneles.Length)
        {
            Debug.LogError("Error el indice es incorrecto...");
            return;
        }
        if (panelActual != null)
        {
            panelActual.Ocultar();
        }

        paneles[indice].Mostrar();
        panelActual = paneles[indice];
    }

    public void OcultarPaneles()
    {
        for (int i = 0; i < paneles.Length; i++)
        {
            paneles[i].Ocultar();
        }
    }

    public void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}