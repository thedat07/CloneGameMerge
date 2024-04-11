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

    public bool fly;

    public float ScaleModel() => sr.size.x / 7;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
    }
    public void Fly()
    {
        rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        fly = true;
        transform.SetParent(null);
        SetSimulated(true);
    }

    public Sequence Init(float time, Ease ease)
    {
        rigid.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOScale(Vector3.one * scale, time).From(Vector3.zero).SetEase(ease));
        return mySequence;
    }

    public Sequence Scale(float time, Ease ease)
    {
        transform.localScale = Vector3.one * scale;
        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOScale(Vector3.one * scale, time).From(Vector3.zero).SetEase(ease)).OnComplete(() =>
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
            rigid.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        });
        return mySequence;
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

    public void SetSimulated(bool b)
    {
        rigid.simulated = b;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (fly == true)
        {
            rigid.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            fly = false;
        }
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
        if (!(transform.localScale == Vector3.one * scale)) return;
        var obj = collision.gameObject;
        if (obj.CompareTag("Redline"))
        {
            isTouchRedline = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!(transform.localScale == Vector3.one * scale)) return;
        var obj = collision.gameObject;
        if (obj.CompareTag("Redline"))
        {
            isTouchRedline = false;
            timer = 0;
        }
    }
}
