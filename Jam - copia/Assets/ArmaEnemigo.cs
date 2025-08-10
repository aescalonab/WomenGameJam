using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArmaEnemigo : MonoBehaviour
{
    [SerializeField] GameObject[] bala;
    [SerializeField] JugadorNave jN;
    private int num;
    [SerializeField] GameObject padre;
    [SerializeField] List<GameObject> proyectiles;
    void Start()
    {
        jN = FindObjectOfType<JugadorNave>();
        Agregar(20);
        num=0;
    }
    public void Agregar(int cantidad)
    {
        for (int x = 0; x < cantidad; x++)
        {
            GameObject b = Instantiate(bala[num], padre.transform);
            ProyectiEnemigo p = b.GetComponent<ProyectiEnemigo>();
            p.j = jN;
            if (num < bala.Length - 1) num++;
            else num = 0;
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
