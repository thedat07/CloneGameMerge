using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Game : MonoBehaviour
{
    [Header("Data")]
    public List<Fruit> fruitPrefabList;
    public List<Sprite> lstSprite;
    public ParticleSystem effect;

    [Header("Ref")]
    public Transform spawnPoint;
    public CanvasGroup uiEnd;
    public SpriteRenderer lineSpriteRenderer;
    public TextMeshProUGUI scoreLabel;

    [Header("Setting")]
    public Vector2 limit;
    public Image imgNext;
    public CanvasScaler canvasScaler;

    private Fruit fruit;
    private int fruidId;
    private bool isGameOver;
    // private bool isSpawn;

    public List<int> lstIdSpawn = new List<int>();
    private List<Fruit> fruits = new List<Fruit>();

    // Start is called before the first frame update
    void Start()
    {
        uiEnd.gameObject.SetActive(false);
        canvasScaler.matchWidthOrHeight = LB.ScaleCanvas();
        lstIdSpawn.Add(Random.Range(0, fruitPrefabList.Count));
        lstIdSpawn.Add(Random.Range(0, fruitPrefabList.Count));
        fruit = SpawnNextFruit();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (fruit.check==false)
            {
                return;
            }
            var mousePos = Input.mousePosition;
            var wolrdPos = Camera.main.ScreenToWorldPoint(mousePos);
            spawnPoint.DOMoveX(Mathf.Clamp(wolrdPos.x, limit.x, limit.y), 0.1f).OnComplete(() =>
            {
                fruit.gameObject.transform.position = spawnPoint.position;
                fruit.transform.parent = null;
                fruit.SetSimulated(true);
                fruit = SpawnNextFruit();
            });
        }
        else
        {
            var mousePos = Input.mousePosition;
            var wolrdPos = Camera.main.ScreenToWorldPoint(mousePos);
            spawnPoint.DOMoveX(Mathf.Clamp(wolrdPos.x, limit.x, limit.y), 0.1f);
        }
    }

    private Fruit SpawnNextFruit()
    {
        var rand = lstIdSpawn.First();
        lstIdSpawn.Remove(rand);
        lstIdSpawn.Add(Random.Range(0, fruitPrefabList.Count));
        var prefab = fruitPrefabList[rand].gameObject;
        var pos = spawnPoint.position;
        imgNext.sprite = lstSprite[lstIdSpawn.First()];
        float size = 180 / (imgNext.sprite.texture.width * 1.0f);
        imgNext.GetComponent<RectTransform>().sizeDelta = new Vector2(imgNext.sprite.texture.width, imgNext.sprite.texture.height) * size;
        imgNext.transform.DOScale(Vector3.one, 0.2f).From(0);
        return SpawnFruit(prefab, pos, true);
    }

    private Fruit SpawnFruit(GameObject prefab, Vector3 pos, bool spawn = false)
    {
        Fruit f;

        if (spawn)
        {
            var obj = Lean.Pool.LeanPool.Spawn(prefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            f = obj.GetComponent<Fruit>();
            lineSpriteRenderer.DOFade(1, 0.25f).From(0).SetDelay(0.25f);
            f.Init();
            f.SetSimulated(false);
            f.check = false;
        }
        else
        {
            var obj = Lean.Pool.LeanPool.Spawn(prefab, pos, Quaternion.identity);
            var objEffect = Lean.Pool.LeanPool.Spawn(effect, pos, Quaternion.identity);
            f = obj.GetComponent<Fruit>();
            f.Init();
            objEffect.transform.localScale = Vector3.one * f.ScaleModel();
            Lean.Pool.LeanPool.Despawn(objEffect, 3);
        }

        f.id = fruidId++;

        f.OnLevelUp = (a, b) =>
        {
            if (IsFruitExist(a) && IsFruitExist(b))
            {
                var pos1 = a.gameObject.transform.position;
                var pos2 = b.gameObject.transform.position;
                var pos = (pos1 + pos2) * 0.5f;
                RemoveFruit(a);
                RemoveFruit(b);
                AddScore(a.score);
                var fr = SpawnFruit(a.nextLevelPrefab, pos);
                fr.SetSimulated(true);
            }
        };

        f.OnGameOver = () =>
        {
            if (isGameOver == true)
            {
                return;
            }
            OnGameOver();
        };

        fruits.Add(f);
        return f;
    }

    private void OnGameOver()
    {
        isGameOver = true;
        uiEnd.gameObject.SetActive(true);
        uiEnd.DOFade(1, 0.2f).From(0);
    }

    public void Restart()
    {
        for (int i = 0; i < fruits.Count; i++)
        {
            fruits[i].SetSimulated(false);
            AddScore(fruits[i].score);
            Lean.Pool.LeanPool.Despawn(fruits[i].gameObject);
        }
        fruits.Clear();
        GameManager.Instance.ResetScore();
        uiEnd.gameObject.SetActive(false);
        fruit = SpawnNextFruit();
        scoreLabel.text = "0";
        isGameOver = false;
    }

    private void RemoveFruit(Fruit f)
    {
        for (int i = 0; i < fruits.Count; i++)
        {
            if (fruits[i].id == f.id)
            {
                fruits.Remove(f);
                Lean.Pool.LeanPool.Despawn(f.gameObject);
                return;
            }
        }
    }

    private bool IsFruitExist(Fruit f)
    {
        for (int i = 0; i < fruits.Count; i++)
        {
            if (fruits[i].id == f.id)
            {
                return true;
            }
        }
        return false;
    }

    private void AddScore(int score)
    {
        GameManager.Instance.AddScore(score);
        scoreLabel.text = $"{GameManager.Instance.score}";
    }
}
