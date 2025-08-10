using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CuerpoSerpiente : MonoBehaviour
{
    [SerializeField] Sprite[] s;
    [SerializeField] SpriteRenderer r;
    private void Start()
    {
        r=GetComponent<SpriteRenderer>();
    }
    public void Cambio(Transform g)
    {
        if (transform.localScale.x < 0) transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        if ((Vector2)g.transform.right != (Vector2)transform.right)
        {
            if ((Vector2)g.transform.right == Vector2.right&& (Vector2)transform.right==Vector2.up)
            {
                r.sprite = s[1];
                transform.right = Vector2.right;
            }
            if ((Vector2)g.transform.right == Vector2.right && (Vector2)transform.right == Vector2.down)
            {
                r.sprite = s[2];
                transform.right = Vector2.right;
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
            if ((Vector2)g.transform.right == Vector2.left && (Vector2)transform.right == Vector2.up)
            {
                r.sprite = s[1];
                transform.right = Vector2.left;
            }
            if ((Vector2)g.transform.right == Vector2.left && (Vector2)transform.right == Vector2.down)
            {
                r.sprite = s[2];
                transform.right = Vector2.left;
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
            if ((Vector2)g.transform.right == Vector2.down&& (Vector2)transform.right == Vector2.right)
            {
                r.sprite = s[1];
                transform.right = Vector2.down;
            }
            if ((Vector2)g.transform.right == Vector2.down && (Vector2)transform.right == Vector2.left)
            {
                r.sprite = s[2];
                transform.right = Vector2.down;
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
            if ((Vector2)g.transform.right == Vector2.up&& (Vector2)transform.right == Vector2.right)
            {
                r.sprite = s[2];
                transform.right = Vector2.up;
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
            if ((Vector2)g.transform.right == Vector2.up && (Vector2)transform.right == Vector2.left)
            {
                r.sprite = s[1];
                transform.right = Vector2.up;
            }
        }
        else
        {
            r.sprite = s[0];
        }

    }
}
