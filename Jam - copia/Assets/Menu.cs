using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
public class Menu : MonoBehaviour
{
    [SerializeField] bool fadeInicio;
    [SerializeField] bool fadeFinal;
    [SerializeField] Image panel;
    float org;
    private void Awake()
    {
        if(fadeInicio)
        {
            Color a=panel.color;
            a.a = 1;
            panel.color = a;
            panel.DOFade(0, 0.5f).SetDelay(0.5f);
        }
    }
    public void Siguiente()
    {
        StartCoroutine(Sig());
    }
    public void Reinicio()
    {
        StartCoroutine(Reini());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reini());
            this.enabled = false;
        }
    }
    public void Salir()
    {
        Application.Quit();
    }
    IEnumerator Sig()
    {
        if (fadeFinal) panel.DOFade(1, 0.75f);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator Reini()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
