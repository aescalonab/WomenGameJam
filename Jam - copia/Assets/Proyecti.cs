using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyecti : MonoBehaviour
{
    [SerializeField] Rigidbody2D r;
    public JugadorNave j;
    [SerializeField] float fuerza;
    [SerializeField] const string d = "Destruir",e= "Enemigo";
    private void OnEnable()
    {
        StartCoroutine(Disparar());
    }
    IEnumerator Disparar()
    {
        yield return new WaitForSeconds(0.1f);
        r.AddForce(Vector2.up * fuerza, ForceMode2D.Impulse);
        yield return new WaitForSeconds(3);
        Destruir();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(e))
        {
            Naves n = collision.GetComponent<Naves>();
            if (n != null) n.estallar();
           j.Puntos();
        }
        Destruir();
    }
    public void Destruir()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }
}
