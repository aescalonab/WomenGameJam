using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Code.Audio;
public class JugadorPlataforma : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Menu menu;
    [SerializeField] List<Image> corazones;
    [SerializeField] SpriteRenderer jugador;
    [SerializeField] ParticleSystem particulas;
    [SerializeField] TextMeshProUGUI []t;
    [SerializeField] GameObject teclas;
    [SerializeField] float pixel;
    [Header("Estadisticas")]
    [SerializeField] int vida;
    [SerializeField] int [] moneda;
    [SerializeField] Guardado g;
    private float inmune;
    [Header("Movimiento")]
    [SerializeField] Animator an;
    [SerializeField] Rigidbody2D rd;
    [SerializeField] Transform a;
    [SerializeField] Transform b;
    [SerializeField] Transform bases;
    [SerializeField] float vel;
    [SerializeField] float velAire;
    [SerializeField] float limVel;
    [SerializeField] float fuerzaSalto;
    [SerializeField] float gravedad;
    [SerializeField] float aceleracion;
    [SerializeField] float desaceleracion;
    private float gravedadOrg;
    private float dir;
    [SerializeField] float fuerzaGravedad;
    [SerializeField] float velGravedad;
    [SerializeField] float disDet;
    [SerializeField] bool contacto;
    [SerializeField] LayerMask piso;
    [Header("Camara")]
    [SerializeField] Transform cam;
    [SerializeField] float velCamara;
    [SerializeField] float altura;
    [SerializeField] bool transicion = false;
    const string m = "Moneda", l = "Lava", ban = "Bandera", te = "Teclas";

    void Start()
    {
        an.SetLayerWeight(1, pixel);
        if (g.teclas)
        {
            teclas.SetActive(true);
            teclas.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.5f).SetLoops(20, LoopType.Yoyo).SetEase(Ease.OutSine).OnUpdate(MoverTeclas).OnComplete(()=> teclas.SetActive(false));
            g.teclas = false;
        }
        g.teclas = false;
        if (transicion == g.transicion) cam.transform.DOMove(new Vector3(Mathf.Clamp(transform.position.x, -3, 27), Mathf.Clamp(transform.position.y, 0, 70), cam.position.z), 8)
                .SetEase(Ease.Linear).SetDelay(1).OnComplete(Transc);
        else
        {
            transicion = false;
            cam.position= new Vector3(Mathf.Clamp(transform.position.x, -3, 27), Mathf.Clamp(transform.position.y, 0, 70), cam.position.z);
        }
        gravedad = 0;
        gravedadOrg = gravedad;
        menu = FindObjectOfType<Menu>();
    }
    public void Transc()
    {
        transicion = false;
        g.transicion = false;
    }
    public void MoverTeclas()
    {
        teclas.transform.position = transform.position + new Vector3(0, 4, 0);
    }
    void Update()
    {
        contacto = Physics2D.Raycast(a.position, Vector2.down, disDet,piso) || Physics2D.Raycast(b.position, Vector2.down, disDet, piso) ? true:false;
        if (contacto)
        {
            if (Vector3.Distance(bases.position, transform.position) > 1) bases.position = transform.position;
            else bases.position = Vector3.Lerp(bases.position, transform.position, 3 * Time.deltaTime);
        }           
        if(Input.GetKeyDown(KeyCode.Space)&&contacto)
        {
            Salto();
        }
        float f = transform.position.y < 0 ? 0 : transform.position.y;
        if (Input.GetKey(KeyCode.W)) altura = f + 5;
        else if (Input.GetKey(KeyCode.S)) altura = f - 5;
        else altura = transform.position.y;
        an.SetBool("Salto", !contacto);
        if (inmune > 0) inmune -= Time.deltaTime;
    }
    private void FixedUpdate()
    {
        Gravedad();
        Vector3 rota = new Vector3(Mathf.Clamp(transform.position.x,-3,27), Mathf.Clamp(altura,0, 70), cam.position.z);
        if(!transicion)cam.position = Vector3.Lerp(cam.position, rota, velCamara * Time.fixedDeltaTime);
        float x = Input.GetAxisRaw("Horizontal");
        Vector2 dr = new Vector2(x, 0);
        if (inmune > 0) return;
        if (contacto && x != 0) dir = Mathf.Lerp(dir, x * vel, aceleracion * Time.fixedDeltaTime);
        if (contacto && x == 0) dir = Mathf.Lerp(dir, 0, desaceleracion * Time.fixedDeltaTime);
        if (!contacto && x != 0) dir = Mathf.Lerp(dir, x * vel, velAire * Time.fixedDeltaTime);
        rd.velocity = new Vector2(dir, rd.velocity.y);
        if (rd.velocity.magnitude > limVel) rd.velocity = Vector2.ClampMagnitude(rd.velocity, limVel);
        if (x < 0) jugador.transform.localScale = new Vector2(-0.75f, 0.75f);
        else if (x > 0) jugador.transform.localScale = new Vector2(0.75f, 0.75f);
        an.SetFloat("Mover",dr.magnitude);
    }
   
    public void Gravedad()
    {
        if (!contacto && rd.velocity.y != 0)
        {
            gravedad = Mathf.Lerp(gravedad, fuerzaGravedad, velGravedad * Time.fixedDeltaTime);
            rd.velocity=new Vector2(rd.velocity.x,rd.velocity.y+gravedad);
        }
        else gravedad = gravedadOrg;
    }
    public void Paso()
    {
        AudioManager.PlayAudio(AudioID.Caminar, this);
    }
    public void Salto()
    {
        rd.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        AudioManager.PlayAudio(AudioID.Jump, this);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(m))
        {
            collision.gameObject.SetActive(false);
            TipoDeColeccionable top = collision.GetComponent<TipoDeColeccionable>();
            if(t!=null)
            {
               for(int a=0;a<t.Length;a++)
                {
                    if(top.tipo==a)
                    {
                        moneda[a]++;
                        t[a].text = "x" + moneda[a].ToString();
                        AudioManager.PlayAudio(AudioID.Tomar, this);
                        break;
                    }
                    
                }
            }
        }
        if (collision.gameObject.CompareTag(ban))
        {
            Bandera bandera = collision.GetComponent<Bandera>();
            if (bandera != null) bandera.Conseguido();
            AudioManager.PlayAudio(AudioID.Bandera, this);
            menu.Siguiente();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(l))
        {
            QuitarVida((collision.transform.position-transform.position).normalized);
        }
    }
    public void QuitarVida(Vector2 dir)
    {
        if (inmune > 0) return;
        AudioManager.PlayAudio(AudioID.Golpe, this);
        inmune = 0.5f;
        transform.position = bases.position;
        rd.velocity = Vector2.zero;
        vida--;
        jugador.DOFade(0, 0.1f).SetLoops(4, LoopType.Yoyo);
        particulas.Play();
        particulas.gameObject.transform.position = corazones[vida].transform.position;
        corazones[vida].gameObject.SetActive(false);
        cam.transform.DOShakePosition(0.2F, 1, 20, 90, false, true, ShakeRandomnessMode.Harmonic);
        int x = dir.x > 0 ? 1 : -1;
     
        if (vida <= 0)
        {
            menu.Reinicio();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(b.position,(Vector2)b.position+Vector2.down*disDet);
        Gizmos.DrawLine(a.position, (Vector2)a.position + Vector2.down * disDet);
    }
}
