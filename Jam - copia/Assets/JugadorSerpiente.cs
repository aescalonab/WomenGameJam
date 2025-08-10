using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JugadorSerpiente : MonoBehaviour
{
    [SerializeField] List<Transform> serpiente;
    [SerializeField] List<CuerpoSerpiente> serpienteLista;
    [SerializeField] bool invertirEjes;
    [SerializeField] Transform cam;
    [SerializeField] Image[] v;
    [SerializeField] ParticleSystem par;
    [SerializeField] int vidas = 3;
    [SerializeField] GameObject referencia;
    [SerializeField] Transform padre;
    [SerializeField] float vel;
    [SerializeField] float inmune;
    private Vector2 org;
    [SerializeField] float distancia;
    private float x, y;
    private float velOrg;
    void Start()
    {
        velOrg = vel;
        for (int i = 0; i < serpiente.Count; i++)
        {
            CuerpoSerpiente s = serpiente[i].GetComponent<CuerpoSerpiente>();
            if(s!=null)serpienteLista.Add(s);
        }
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);
        if(!invertirEjes)
        {
            if (x >= 1) org = Vector2.right;
            else if (x <= -1) org = Vector2.left;
            else if (y >= 1) org = Vector2.up;
            else if (y <= -1) org = Vector2.down;
        }
        else
        {
            if (x >= 1) org = Vector2.left;
            else if (x <= -1) org = Vector2.right;
            else if (y >= 1) org = Vector2.down;
            else if (y <= -1) org = Vector2.up;
        }
        if (vel > 0) vel -= Time.deltaTime;
       
        if (vel <= 0)
        {
            vel = velOrg;
            Dirrecion(org);
        }
        if (inmune > 0) inmune -= Time.deltaTime;
    }
    public void MasPiezas()
    {
        GameObject b = Instantiate(referencia, padre.transform);
        b.transform.right = serpiente[0].transform.right;
        b.transform.position = serpiente[0].transform.position;
        CuerpoSerpiente s = b.GetComponent<CuerpoSerpiente>();
        serpiente.Insert(1, b.transform);
        serpienteLista.Insert(0, s);
        serpiente[0].transform.position = (Vector2)serpiente[0].position + -(Vector2)serpiente[0].transform.right * distancia;
    }
    public void Golpe()
    {
        if (inmune > 0) return;
        for(int x=0;x<serpiente.Count;x++)
        {
            SpriteRenderer r = serpiente[x].GetComponent<SpriteRenderer>();
            r.DOFade(0, 0.1f).SetLoops(4, LoopType.Yoyo);
        }
        cam.transform.DOShakePosition(0.2F, 1, 20, 90, false, true, ShakeRandomnessMode.Harmonic);
        vidas--;
        v[vidas].gameObject.SetActive(false);
        par.Play();
        par.gameObject.transform.position = v[vidas].transform.position;
        if (vidas <= 0)
        {
            StartCoroutine(Reinicio());
        }
    }
    public void Dirrecion(Vector2 dir)
    {
        for (int x = 0; x < serpiente.Count; x++)
        {
            
            if (x == serpiente.Count - 1)
            {
                if (serpiente[x].position.x > 8.5f || serpiente[x].position.x < -8.5f)
                {
                    serpiente[x].position = new Vector2(Mathf.Clamp(serpiente[x].position.x, -8.5f, 8.5f) * -1, serpiente[x].position.y);
                    Actualizar();
                    break;
                }

                if (serpiente[x].position.y > 4.5f || serpiente[x].position.y < -4.5f)
                {
                    serpiente[x].position = new Vector2(serpiente[x].position.x, Mathf.Clamp(serpiente[x].position.y, -4.5f, 4.5f) * -1);
                    Actualizar();
                    break;
                }
                if (dir != -(Vector2)serpiente[x].transform.right) serpiente[x].transform.right = dir;
                serpiente[x].position = (Vector2)serpiente[x].position + (Vector2)serpiente[x].transform.right * distancia;
                Actualizar();
                break;
            }
            if(x==0) serpiente[x].transform.right = serpiente[x + 1].transform.right;
            serpiente[x].position = serpiente[x + 1].position;
        }
    }
    public void Actualizar()
    {
        for(int x=0;x<serpienteLista.Count;x++)
        {
            serpienteLista[x].Cambio(serpiente[x + 2]);
        }
    }
  
    IEnumerator Reinicio()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
