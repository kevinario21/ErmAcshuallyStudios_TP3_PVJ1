using UnityEngine;
using UnityEngine.UI;

public class MenuOpciones : UIPanel
{
    [SerializeField] private Button btnVolver;
    [SerializeField] private GestorUI gestor;

    [Header("Configuración de Paneles")]
    [SerializeField] private int indiceMenuPrincipal = 0; 

    public override void Mostrar()
    {
        gameObject.SetActive(true);
        if (gestor == null)
        {
            Debug.LogError("El GestorUI no se asignó a este panel de opciones.");
            return;
        }

        // Botón Volver: Regresa al menú principal usando el índice del GestorUI
        btnVolver.onClick.RemoveAllListeners();
        btnVolver.onClick.AddListener(() => gestor.MostrarPaneles(indiceMenuPrincipal));
    }

    public override void Ocultar()
    {
        gameObject.SetActive(false);
    }
}