using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject piedra;           
    public float primerDelay = 2f;      
    public float intervalo = 6f;        
    public float vida = 6f;            
    public float velocidadInicial = 0f; 

    void Start()
    {
        InvokeRepeating("CrearPiedra", primerDelay, intervalo);
    }

    void CrearPiedra()
    {
        GameObject nueva = Instantiate(piedra, transform.position, Quaternion.identity);

        Rigidbody rbPiedra = nueva.GetComponent<Rigidbody>();
        rbPiedra.linearVelocity = transform.forward * velocidadInicial;

        Destroy(nueva, vida);
    }
}