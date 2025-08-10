using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
public class JugadorPlataforma : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] List<Image> corazones;
    [SerializeField] SpriteRenderer jugador;
    [SerializeField] ParticleSystem particulas;
    [SerializeField] TextMeshProUGUI t;
    [SerializeField] GameObject teclas;
    [Header("Estadisticas")]
    [SerializeField] int vida;
    [SerializeField] int moneda;
    [SerializeField] float fuerzaEmpuje;
    private float inmune;
    [Header("Movimiento")]
    [SerializeField] Animator an;
    [SerializeField] Rigidbody2D rd;
    [SerializeField] Transform a;
    [SerializeField] Transform b;
    [SerializeField] float vel;
    [SerializeField] float velAire;
    [SerializeField] float limVel;
    [SerializeField] float fuerzaSalto;
    [SerializeField] float gravedad;
    private float gravedadOrg;
    [SerializeField] float fuerzaGravedad;
    [SerializeField] float deslizar;
    [SerializeField] float disDet;
    [SerializeField] bool contacto;
    [SerializeField] LayerMask piso;
    [Header("Camara")]
    [SerializeField] Transform cam;
    [SerializeField] float velCamara;
    [SerializeField] float altura;
    const string m = "Moneda", l = "Lava", ban = "Bandera", te = "Teclas";

    void Start()
    {
        Invoke(te, 10);
        teclas.transform.DOScale(new Vector3(0.8f,0.8f,0.8f), 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutSine).OnUpdate(MoverTeclas);
        moneda = 0;
        gravedad = rd.gravityScale;
        gravedadOrg = gravedad;
    }
    public void MoverTeclas()
    {
        teclas.transform.position = transform.position + new Vector3(0, 4, 0);
    }
    public void Teclas()
    {
        teclas.SetActive(false);
    }

    void Update()
    {
        contacto = Physics2D.Raycast(a.position, Vector2.down, disDet,piso) || Physics2D.Raycast(b.position, Vector2.down, disDet, piso) ? true:false;
        if(Input.GetKeyDown(KeyCode.Space)&&contacto)
        {
            Salto();
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) altura = transform.position.y + 5;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) altura = transform.position.y - 5;
        else altura = transform.position.y;
            an.SetBool("Salto", !contacto);
        if (inmune > 0) inmune -= Time.deltaTime;
    }
    private void FixedUpdate()
    {
        Gravedad();
        Vector3 rota = new Vector3(Mathf.Clamp(transform.position.x,-3,27), Mathf.Clamp(altura,0, 70), cam.position.z);
        cam.position = Vector3.Lerp(cam.position, rota, velCamara * Time.fixedDeltaTime);
        if (contacto)rd.drag = deslizar;
        else rd.drag = 0;
        float x = Input.GetAxis("Horizontal");
        Vector2 dir = new Vector2(x, 0).normalized;
        if (dir != Vector2.zero)
        {
            if (contacto) rd.AddForce(dir * vel, ForceMode2D.Force);
            else rd.AddForce(dir * vel * velAire, ForceMode2D.Force);
        }
        if (rd.velocity.magnitude > limVel) rd.velocity = Vector2.ClampMagnitude(rd.velocity, limVel);
        if (x < 0) jugador.transform.localScale = new Vector2(-0.75f, 0.75f);
        else if (x > 0) jugador.transform.localScale = new Vector2(0.75f, 0.75f);
        an.SetFloat("Mover", dir.magnitude);
    }
    public void Gravedad()
    {
        if (!contacto && rd.velocity.y != 0)
        {
            gravedad += Time.fixedDeltaTime * fuerzaGravedad;
            rd.AddForce(Vector2.down * gravedad);
        }
        else gravedad = gravedadOrg;
    }
    public void Salto()
    {
        rd.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(m))
        {
            collision.gameObject.SetActive(false);
            moneda++;
            t.text = "x" + moneda.ToString();
        }
        if (collision.gameObject.CompareTag(ban))
        {
            Bandera bandera = collision.GetComponent<Bandera>();
            if (bandera != null) bandera.Conseguido();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(l)&&inmune<=0)
        {
            QuitarVida((collision.transform.position-transform.position).normalized);
        }
    }
    public void QuitarVida(Vector2 dir)
    {
        inmune = 1f;
        vida--;
        jugador.DOFade(0, 0.1f).SetLoops(4, LoopType.Yoyo);
        particulas.Play();
        particulas.gameObject.transform.position = corazones[vida].transform.position;
        corazones[vida].gameObject.SetActive(false);
        cam.transform.DOShakePosition(0.2F, 1, 20, 90, false, true, ShakeRandomnessMode.Harmonic);
        int x = dir.x > 0 ? 1 : -1;
        rd.AddForce(new Vector2(fuerzaEmpuje*x, fuerzaEmpuje), ForceMode2D.Impulse);
        if (vida <= 0)
        {
            StartCoroutine(Reinicio());
        }
    }
    IEnumerator Reinicio()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(b.position,(Vector2)b.position+Vector2.down*disDet);
        Gizmos.DrawLine(a.position, (Vector2)a.position + Vector2.down * disDet);
    }
}
