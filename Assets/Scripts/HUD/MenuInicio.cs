using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuInicio : UIPanel
{
    [SerializeField] private Button btnInicio;
    [SerializeField] private Button btnOpciones;
    [SerializeField] private Button btnSalir;
    [SerializeField] private GestorUI gestor;

    [Header("Configuración de Paneles")]
    [SerializeField] private int indicePanelOpciones = 1; // Cambia este número según la posición del panel de opciones en tu GestorUI

    public override void Mostrar()
    {
        gameObject.SetActive(true);
        if (gestor == null)
        {
            Debug.LogError("El GestorUI no se asigno a este panel->");
            return;
        }

        // Botón Inicio: Carga la escena del primer nivel (reemplaza "Juego" por el nombre real de tu escena)
        btnInicio.onClick.RemoveAllListeners();
        btnInicio.onClick.AddListener(() => SceneManager.LoadScene("Tutorial"));

        // Botón Opciones: Usa el GestorUI para cambiar al panel de opciones usando su índice numérico
        btnOpciones.onClick.RemoveAllListeners();
        btnOpciones.onClick.AddListener(() => gestor.MostrarPaneles(indicePanelOpciones));

        // Botón Salir: Llama a la función Salir() que ya tienes creada en tu GestorUI
        btnSalir.onClick.RemoveAllListeners();
        btnSalir.onClick.AddListener(() => gestor.Salir());
    }

    public override void Ocultar()
    {
        gameObject.SetActive(false);
    }
}