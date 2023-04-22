using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameResult : MonoBehaviour
{
    public Text PlayerWinLabel;
    public Text PlayerDefeatedLabel;
    public Text PlayerWinScoreLabel;
    public Text PlayerDefeatedScoreLabel;

    public GameObject PlayerWin;
    public GameObject PlayerDefeated;

    public Text OponentWinLabel;
    public Text OponentLooseLabel;
    public Text OponentWinScoreLabel;
    public Text OponentDefeatedScoreLabel;

    public GameObject OponentWin;
    public GameObject OponentDefeated;

    public UIManager m_uimanager;
    public int timeLeft = 20;
    public string PlayerName;
    public GameObject ResultEnd;
    public GameObject Menu;

    public Text Timer;

    void OnEnable()
    {
        StartCoroutine("ResultTime");
    }

    // Update is called once per frame
    void Update()
    {
        Timer.text = ("" + timeLeft);
        if (timeLeft > 20)
        {
            Timer.text = ("20");
        }
    }

    public delegate void ResultExitHandler();
    public event ResultExitHandler ResultExit;

    IEnumerator ResultTime()
    {
        bool run = true;
        while (run)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;
            if (timeLeft < 0)
            {
                Menu.SetActive(true);
                run = false;
                ResultEnd.SetActive(false);
                timeLeft = 20;

                if(ResultExit!=null)
                {
                    ResultExit();
                }

                yield break;
                //  SceneManager.LoadScene("GUI");
            }

        }

    }
    public void ReturnButton()
    {
        timeLeft = 0;

    }
}
