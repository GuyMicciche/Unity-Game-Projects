using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameStart()
    {
        UIManager.Instance.GameStart();
        ScoreManager.Instance.StartScore();
        GameObject.Find("PlatformSpawner").GetComponent<PlatformSpawner>().StartSpawningPlatforms();
    }

    public void GameOver()
    {
        UIManager.Instance.GameOver();
        ScoreManager.Instance.StopScore();
    }
}
