using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Android;

public class GameManager : Singleton<GameManager>
{
    public int score;

    public void AddScore(int score)
    {
        this.score += score;
    }

    public void ResetScore()
    {
        this.score = 0;
    }

    public int GetSorce()
    {
        int max = PlayerPrefs.GetInt("MaxScore") >= score ? PlayerPrefs.GetInt("MaxScore") : score;
        PlayerPrefs.SetInt("MaxScore", max);
        PlayerPrefs.Save();
        return max;
    }
}
