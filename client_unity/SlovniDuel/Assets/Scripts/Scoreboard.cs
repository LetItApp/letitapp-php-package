using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    public GameConnection m_gameConnection;
    public GameObject ScorePrefab;
    public GameObject ScoresContainer;
    public GameObject Trophy;

    void Start()
    {
        
    }

    void OnEnable()
    {
        List<ScoreboardPlayer> scoreboard = m_gameConnection.GetScoreBoard();
        int index = 0;
        double lastScore = double.MaxValue;

        Color yellowColor = new Color(0.9686f, 0.8588f, 0.0470f);

        bool first = true;

        foreach (Transform child in ScoresContainer.transform)
        {
            if (child.name != "PrefabScore")
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        foreach (ScoreboardPlayer p in scoreboard)
        {
            GameObject item = Instantiate(ScorePrefab, ScoresContainer.transform, true);
            item.SetActive(true);
            
            bool yellow = first || p.me;
            first = false;

            if (p.score < lastScore)
            {
                index++;
                lastScore = p.score;
            }

            foreach (Transform child in item.transform)
            {
                if (child.name == "Pozice")
                {
                    child.GetComponent<Text>().text = index.ToString() + ".";
                    if (yellow)
                    {
                        child.GetComponent<Text>().color = yellowColor;
                    }
                    if (index!=0)
                    {
                        Trophy.SetActive(false);
                    }
                }
                else
                if(child.name == "Nick")
                {
                    child.GetComponent<Text>().text = p.nick;

                    if (yellow)
                    {
                        child.GetComponent<Text>().color = yellowColor;
                    }
                }
                else
                if (child.name == "Score")
                {
                    child.GetComponent<Text>().text = p.score.ToString();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
