using UnityEngine;

public class MenuPausa : UIPanel
{
    public override void Mostrar()
    {
        gameObject.SetActive(true);
        Debug.Log("[MenuPausa] ¡Panel de pausa activado visualmente!");
    }

    public override void Ocultar()
    {
        gameObject.SetActive(false);
    }
}