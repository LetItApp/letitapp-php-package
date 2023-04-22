using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartTimer : MonoBehaviour
{
    public GameObject StartTimerObj;
    public GameObject BlockObj;
    public Text StartTimerLabel;
    public GamePlay gameplay;
    public int Count = 7;
    public int aa=1;

    public void SetTime(int time)
    {
        Count = time;
    }

    public void PrepareTimer()
    {
        BlockObj.SetActive(true);
    }

    /*
    void OnEnable()
    {
        Time.timeScale = 1;
        StartCoroutine("Starter");
        BlockObj.SetActive(true);
}
*/

    // Update is called once per frame
    void Update()
    {
        if (Count == 6)
        {
            StartTimerLabel.text = "5";
        }
        if (Count==5)
        {           
            StartTimerLabel.text = "4";
        }
       else if (Count == 4)
        {
            StartTimerLabel.text = "3";
        }
        else if (Count == 3)
        {
            StartTimerLabel.text = "2";
        }
        else if (Count == 2)
        {
            StartTimerLabel.text = "1";
        }
        else if (Count == 1)
        {
            StartTimerLabel.fontSize = 180;
            StartTimerLabel.text = "Start!";
            BlockObj.SetActive(false);
        }
        else if (Count == 0)
        {
            aa = 0;
            StartTimerLabel.fontSize = 180;
            StartTimerLabel.text = "";

            TimerFinish(); 
        }

    }
    
    public void TimerFinish()
    {
        StartTimerObj.SetActive(false);
        //gameplay.StartGameCountDown();
        Count = 7;
    }

    /*
     IEnumerator Starter()
      {
          while (true)
          {
              yield return new WaitForSeconds(1);
            Count--;
              if (Count < 0)
              {
                  StartTimerObj.SetActive(false);               
                Time.timeScale = 1;
                gameplay.StartGameCountDown();
                Count = 7;
                yield break;
            }
        }
      } */
}
