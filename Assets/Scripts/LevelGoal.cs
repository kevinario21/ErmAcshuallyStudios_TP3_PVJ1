using UnityEngine;
using System.Collections;

// Meta del nivel (Collider con Is Trigger). Al entrar el jugador, el GameManager
// pasa al siguiente nivel conservando los datos, o gana si era el último.
public class LevelGoal : MonoBehaviour
{
    private bool reached;

    [SerializeField] private GameObject goalEffect;
    [SerializeField] private float celebrationDelay = 2f; 
   
    private void OnTriggerEnter(Collider other)
    {
        if (reached) return;
        if (other.GetComponentInParent<PlayerMovement>() == null) return;
        if (GameManager.Instance == null) return;

        reached = true;
       
        StartCoroutine(VictoryCelebration());
    }

    //Cuando llega a la meta se activan las partículas de celebración y se espera un tiempo antes de pasar al siguiente nivel.
    IEnumerator VictoryCelebration()
    {
        goalEffect.SetActive(true);
        yield return new WaitForSeconds(celebrationDelay);

        GameManager.Instance.CompleteLevel();

    }
}
