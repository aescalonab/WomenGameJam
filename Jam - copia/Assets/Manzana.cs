using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manzana : MonoBehaviour
{
    [SerializeField] float vel = 0.5f;
    [SerializeField] float distancia = 0.5f;
    [SerializeField] CabezaSerpiente c;
    private float velOrg;
    void Start()
    {
        c=FindObjectOfType<CabezaSerpiente>();
        velOrg=vel;
    }

    // Update is called once per frame
    void Update()
    {
       if(vel<=0)
        {
            vel = velOrg;
            transform.up = (c.transform.position - transform.position).normalized;
            transform.position = transform.position+ transform.up * distancia;
        }
        if (vel > 0) vel -= Time.deltaTime;
    }
}
