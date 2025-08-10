using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandera : MonoBehaviour
{
    [SerializeField] Animator an;
    public void Conseguido()
    {
        an.SetTrigger("Conseguir");
    }
}
