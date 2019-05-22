using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject MainPanel;
    public GameObject GameOverPanel;
    public GameObject TapStart;
    public Text Score;
    public Text HighScore1;
    public Text HighScore2;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start()
    {
        HighScore1.text = "HIGH SCORE: " + PlayerPrefs.GetInt("HighScore").ToString();
    }

    public void GameStart()
    {
        TapStart.SetActive(false);
        MainPanel.GetComponent<Animator>().Play("MainPanelUp");
    }

    public void GameOver()
    {
        Score.text = "SCORE: " + PlayerPrefs.GetInt("Score").ToString();
        HighScore2.text = "HIGH SCORE: " + PlayerPrefs.GetInt("HighScore").ToString();

        GameOverPanel.SetActive(true);
    }

    public void Reset()
    {
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
