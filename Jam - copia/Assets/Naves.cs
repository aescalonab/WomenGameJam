using System;
using System.Collections;
using System.Collections.Generic;
using Code.Audio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Naves : MonoBehaviour
{
    [SerializeField] bool pasivo = true;
    [SerializeField] ArmaEnemigo arm;
    [SerializeField] Animator an;
    [SerializeField] GameObject arma;
    [SerializeField] float vel;
    [SerializeField] Vector2[] mov;
    [SerializeField] int lista;
    [SerializeField] float distancia;
    [SerializeField] JugadorNave jN;
    const string l = "Lava";
    private float velOrg;
    void Start()
    {
        lista = -1;
        velOrg = vel;
        jN=FindObjectOfType<JugadorNave>();
        arm = FindObjectOfType<ArmaEnemigo>();
    }

    // Update is called once per frame
    void Update()
    {
        if(vel<=0)
        {
            vel = velOrg;
            Mover();
        }
        if (vel > 0) vel-=Time.deltaTime;
    }
    public void Mover()
    {
        if (lista < mov.Length - 1) lista++;
        else lista = 0;
        transform.position = (Vector2)transform.position + mov[lista] * distancia;
        if(!pasivo)
        {
            GameObject b = arm.Requimiento();
            b.transform.position = arma.transform.position;
            AudioManager.PlayAudio(AudioID.DisparoEnemigo, this);
        }
    }
    public void estallar()
    {
        StartCoroutine(estalla());
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(l))
        {
            if(pasivo)
            {
                jN.QuitarVida();
            }
            jN.Puntos();
            estallar();
        }
    }
    IEnumerator estalla()
    {
        an.SetTrigger("Boom");
        AudioManager.PlayAudio(AudioID.ExplosionEnemiga, this);
        yield return new WaitForSeconds(0.5f);
        this.gameObject.SetActive(false);
    }
}
