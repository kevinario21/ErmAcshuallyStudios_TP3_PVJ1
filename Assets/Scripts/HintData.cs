using UnityEngine;

// Mensaje configurable de un pista. Create > Hint Data

[CreateAssetMenu(fileName = "New Hint", menuName = "Hint Data")]
public class HintData : ScriptableObject
{
    [TextArea(2, 5)]
    [Tooltip("Texto de la pista que se mostrará en pantalla.")]

    public string hintText;
}
