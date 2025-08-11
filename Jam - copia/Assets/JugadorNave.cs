using System.Collections;
using System.Collections.Generic;
using Code.Audio;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JugadorNave : MonoBehaviour
{
    [SerializeField] Menu menu;
    [SerializeField] Rigidbody2D rd;
    [SerializeField] Animator an;
    [SerializeField] TextMeshProUGUI t;
    [SerializeField] bool pasivo;
    [SerializeField] int puntos;
    [SerializeField] int maxPuntos=25;
    [SerializeField] int vida;
    [SerializeField] SpriteRenderer jugador;
    [SerializeField] Animator[] corazones;
    [SerializeField] float vel;
    [SerializeField] float intervalo;
    [SerializeField] float velDisparo=1;
    [SerializeField] float inmune = 0;
    [SerializeField] Transform cam;
    [SerializeField] Transform arma;
    [SerializeField] List<GameObject> proyectiles;
    [SerializeField] GameObject referencia;
    [SerializeField] GameObject padre;
    private void Start()
    {
        Agregar(20);
        puntos = 0;
        menu = FindObjectOfType<Menu>();
    }
    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        Vector2 dir= new Vector2(x,0).normalized;
        if (dir != Vector2.zero) rd.MovePosition(rd.position + dir * vel * Time.fixedDeltaTime);
        an.SetFloat("Mover", dir.magnitude);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && intervalo <= 0&&!pasivo)
        {
            Disparar();
        }
        if (intervalo > 0) intervalo -= Time.deltaTime;
        if (inmune > 0) inmune -= Time.deltaTime;
    }
    public void Disparar()
    {
        AudioManager.PlayAudio(AudioID.DisparoAliado, this);
        intervalo = velDisparo;
        GameObject b = Requimiento();
        b.transform.position = arma.position;
        Proyecti p = b.GetComponent<Proyecti>();
        p.j = this;
    }
    public void QuitarVida()
    {
        if (inmune > 0) return;
        StartCoroutine(Q());
    }
    IEnumerator Q()
    {
        inmune = 1f;
        vida--;
        AudioManager.PlayAudio(AudioID.GolpeNave, this);
        jugador.DOFade(0, 0.1f).SetLoops(4, LoopType.Yoyo);
        corazones[vida].SetTrigger("Golpe");
        cam.transform.DOShakePosition(0.2F, 1, 20, 90, false, true, ShakeRandomnessMode.Harmonic);
        yield return new WaitForSeconds(0.25f);
        if(vida<=0) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else corazones[vida].gameObject.SetActive(false);
    }
    public void Puntos()
    {
        puntos++;
        t.text = "x" + puntos.ToString();
        if(puntos>maxPuntos) menu.Siguiente();
    }
    public void Agregar(int cantidad)
    {
        for(int x=0;x<cantidad;x++)
        {
            GameObject b = Instantiate(referencia, padre.transform);
            b.SetActive(false);
            proyectiles.Add(b);
        }
    }
    public GameObject Requimiento()
    {
        for (int x = 0; x < proyectiles.Count; x++)
        {
            if (!proyectiles[x].activeSelf)
            {
                proyectiles[x].SetActive(true);
                return proyectiles[x];
            }
        }
        Agregar(1);
        proyectiles[proyectiles.Count - 1].SetActive(true);
        return proyectiles[proyectiles.Count - 1];
    }
}
