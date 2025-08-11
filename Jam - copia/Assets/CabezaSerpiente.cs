using System.Collections;
using System.Collections.Generic;
using Code.Audio;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CabezaSerpiente : MonoBehaviour
{
    [SerializeField] int manzanas;
    [SerializeField] Menu menu;
    [SerializeField] int maxManzanas=10;
    [SerializeField] float tiempo;
    [SerializeField] float tiempoMaximo=60;
    [SerializeField] bool contador;
    [SerializeField] Animator an;
    [SerializeField] TextMeshProUGUI t;
    const string m = "Moneda",l="Lava";
    [SerializeField] JugadorSerpiente jS;
        private void Start()
    {
        an=GetComponent<Animator>();
        jS=GetComponentInParent<JugadorSerpiente>();
        manzanas = 0;
        menu = FindObjectOfType<Menu>();
    }
    private void Update()
    {
        if (!contador) return;
       if( tiempo>tiempoMaximo)
        {
            menu.Siguiente();
            this.enabled = false;
        }
        if (tiempo < tiempoMaximo) tiempo += Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(m))
        {
            collision.gameObject.SetActive(false);
            an.SetTrigger("Comer");
            jS.MasPiezas();
            manzanas++;
            if(manzanas>maxManzanas) menu.Siguiente();
            t.text="x"+manzanas.ToString();
            AudioManager.PlayAudio(AudioID.Comer, this);
        }
        if(collision.gameObject.CompareTag(l))
        {
            jS.Golpe();
            Manzana m = collision.GetComponent<Manzana>();
            if (m!=null) collision.gameObject.SetActive(false);
            AudioManager.PlayAudio(AudioID.GolpeSerpiente, this);
        }
    }
    IEnumerator Reinicio()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
