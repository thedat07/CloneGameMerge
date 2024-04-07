using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
