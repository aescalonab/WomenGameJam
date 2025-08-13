using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Gira : MonoBehaviour
{  
    void Start()
    {
        transform.DORotate(new Vector3(0,0,-360), 10f,RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }
}
