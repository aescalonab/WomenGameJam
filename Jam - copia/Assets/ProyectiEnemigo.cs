using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectiEnemigo : MonoBehaviour
{
    [SerializeField] Rigidbody2D r;
    public JugadorNave j;
    [SerializeField] float fuerza;
    [SerializeField] const string d = "Destruir",p= "Player";
    private void OnEnable()
    {
        StartCoroutine(Disparar());
    }
    IEnumerator Disparar()
    {
        yield return new WaitForSeconds(0.1f);
        r.AddForce(Vector2.down * fuerza, ForceMode2D.Impulse);
        yield return new WaitForSeconds(3);
        Destruir();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(p))
        {
            Destruir();
            j.QuitarVida();
        } 
    }
    public void Destruir()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }
}
