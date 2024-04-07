using System;
using UnityEngine;
using DG.Tweening;

public class Fruit : MonoBehaviour
{
    public int id;
    public int score;
    public float scale = 0.75f;
    public GameObject nextLevelPrefab;
    public Action<Fruit, Fruit> OnLevelUp;
    public Action OnGameOver;
    public SpriteRenderer sr;

    private Rigidbody2D rigid;
    private bool isTouchRedline;
    private float timer;

    public float ScaleModel() => sr.size.x / 4;

    public bool check;

    public void Init()
    {
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (isTouchRedline == false)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer > 3)
        {
            OnGameOver?.Invoke();
        }
    }


    void FixedUpdate()
    {
        var step = 3 * Time.fixedDeltaTime;
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * scale, step);
        if (Vector3.Distance(transform.localScale, Vector3.one * scale) < 0.001f)
        {
            check = true;
        }
        else
        {
            check = false;
        }
    }

    public void SetSimulated(bool b)
    {
        if (rigid == null)
        {
            rigid = GetComponent<Rigidbody2D>();
        }
        rigid.simulated = b;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        var fruit = obj.GetComponent<Fruit>();
        if (obj.CompareTag("Fruit"))
        {
            if (obj.name == gameObject.name)
            {
                if (nextLevelPrefab != null)
                {
                    OnLevelUp?.Invoke(this, fruit);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Redline"))
        {
            isTouchRedline = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Redline"))
        {
            isTouchRedline = false;
            timer = 0;
        }
    }
}
