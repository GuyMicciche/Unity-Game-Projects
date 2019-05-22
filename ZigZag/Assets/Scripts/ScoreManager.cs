using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager Instance;

    public int Score;
    public int HighScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start()
    {
        UpdateScore(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateScore(int? score = null)
    {
        if(score != null)
        {
            Score = score.Value;
        }

        PlayerPrefs.SetInt("Score", Score);
    }

    private void IncrementScore()
    {
        Score += 1;
    }

    public void StartScore()
    {
        InvokeRepeating("IncrementScore", 0.1f, 0.5f);
    }

    public void StopScore()
    {
        CancelInvoke("IncrementScore");
        UpdateScore();

        if(PlayerPrefs.HasKey("HighScore"))
        {
            if(Score > PlayerPrefs.GetInt("HighScore", Score))
            {
                PlayerPrefs.SetInt("HighScore", Score);
            }
        }
        else
        {
            PlayerPrefs.SetInt("HighScore", Score);
        }
    }

}
