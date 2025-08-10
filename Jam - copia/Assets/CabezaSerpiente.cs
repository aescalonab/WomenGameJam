using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CabezaSerpiente : MonoBehaviour
{
    [SerializeField] int manzanas;
    [SerializeField] Animator an;
    [SerializeField] TextMeshProUGUI t;
    const string m = "Moneda",l="Lava";
    [SerializeField] JugadorSerpiente jS;
        private void Start()
    {
        an=GetComponent<Animator>();
        jS=GetComponentInParent<JugadorSerpiente>();
        manzanas = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(m))
        {
            collision.gameObject.SetActive(false);
            an.SetTrigger("Comer");
            jS.MasPiezas();
            manzanas++;
            t.text="x"+manzanas.ToString();
        }
        if(collision.gameObject.CompareTag(l))
        {
            jS.Golpe();
            Manzana m = collision.GetComponent<Manzana>();
            if (m!=null) collision.gameObject.SetActive(false);
        }
    }
    IEnumerator Reinicio()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
