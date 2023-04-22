using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.IO;

public class GamePlay : MonoBehaviour
{
    public StartTimer StarfOfTheGame;
    public GameObject endGame;
    public GameObject Game;
    public GameObject WrongWord;
    public GameObject DuplicitWord;
    public InputField mainInputField;
    public Text textArea;
    public Text ResultPlayertextArea;
    public Text ResultOponenttextArea;

    public Text MyScoreLabel;
    public Text OponentScoreLabel;

    public GameObject PlayerWinName;
    public GameObject OponentWinName;
    public GameObject PlayerDefeatedName;
    public GameObject OponentDefeatedName;

    public GameObject PlayerWin;
    public GameObject PlayerDefeated;
    public GameObject OponentWin;
    public GameObject OponentDefeated;

	public AdScript AdScript;

    private string word;
    public int timeLeft = 45;
    //public TextAsset words;
    public Dictionary dict;
    
    private int myScore = 0;
    private int oponentScore = 0;

    private List<string> oponentWords = new List<string>();
    private List<string> myWords = new List<string>();

    public void OponentWord(string word)
    {
        oponentWords.Add(word);
    }

    public Text Timer;
    public Text AlphabetLabel;
    public Text AlphabetLabelResult;
    public string alpha;
    public int aa;
    public Animation animationTimer;

    public string oponentUUID;

    public string[] Alphabet = new string[] { "A", "B" };
    public void GenerateLetter(int seed) {
        Random.seed = seed;
        string[] Alphabet = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "V", "Z", "Č", "Ř", "Š", "Ž" };
        alpha = (Alphabet[Random.Range(0, Alphabet.Length)]);
        
        AlphabetLabel.text = alpha;
        AlphabetLabelResult.text = alpha;
    }

    private void OnEnable()
    {
       
        PrepareGame();
    }

    void Awake()
    {




        //loading words
        //dict = new Dictionary<string, bool>();
        //string line;
        /*using (var streamReader = new StreamReader(Application.dataPath + "/Resources/Czech.txt"))
        {
            while ((line = streamReader.ReadLine()) != null)
            {
                dict[line.ToLower()] = true;
            }
        }*/

        /*using (var streamReader = new StreamReader(new MemoryStream(words.bytes)))
        {
            while ((line = streamReader.ReadLine()) != null)
            {
                dict[line.ToLower()] = true;
            }
        }*/

        /*string[] linesFromfile = words.text.Split("\n"[0]);
        //Debug.Log(linesFromfile.Length);
        foreach (string line in linesFromfile){
            dict[line.ToLower()] = true;
        }*/
    }
    private bool gameEnded = false;
    public void SetTimeLeft(int seconds)
    {
        timeLeft = seconds;
        if (seconds == 0)
        {
            //GameEnded(); WAWA
        }
    }

    void Update()
    {
        Timer.text = ("" + timeLeft);
        if (timeLeft>45)
        {
            Timer.text = ("45");
        }

    }

    /*  
    public void StartGameCountDown()
    {
        //StartCoroutine("LoseTime");
    }*/

    public bool lastTransactionConfirmed = true;
    public double PremiumWordsLeft = 0;

    public void UseAdvice()
    {
        if (lastTransactionConfirmed && PremiumWordsLeft > 0) {
            PremiumWordsLeft--;
            lastTransactionConfirmed = false;

            string adv = dict.GetAdvise(myWords, alpha.ToLower());
            if (adv != null)
            {
                mainInputField.text = adv;
                Send();
                if (AdviceUsed != null)
                {
                    AdviceUsed();
                }
            }
        }
    }

    public void GameEnded()
    {
        if (gameEnded)
            return;

        gameEnded = true;

		AdScript.RequestFullscreenBanner();

        endGame.SetActive(true);
        Game.SetActive(false);
        //vypocet skore
        string resultOponent = "";
        string resultMe = "";

        string prependWin = "";
        string appendWin = "";
        string prependLoose = "";
        string appendLoose = "";

        prependWin = "<color=#f7db0c>";
        appendWin = "</color>";
        prependLoose = "<color=#8a4bf9>";
        appendLoose = "</color>";

        foreach (string word in oponentWords)
        {
            if (CheckWord(word))
            {
                int points = 5;

                string prepend = "";
                string append = "";

                if (myWords.Contains(word))
                {
                    points = 3;
                    prepend = "<color=#f7db0c>";
                    append = "</color>";
                }
                else
                {
                    /*  prepend = "<color=#f7db0c>";
                      append = "</color>"; */
                }

                oponentScore += points;
                resultOponent += prepend + points + " | " + append + word + "\n";


            }
        }

        //send oponents score
        if (OponentScore != null)
        {
            OponentScore(oponentUUID, oponentScore);
        }

        foreach (string word in myWords)
        {
            if (CheckWord(word))
            {
                int points = 5;

                string prepend = "";
                string append = "";

                if (oponentWords.Contains(word))
                {
                    points = 3;
                    prepend = "<color=#f7db0c>";
                    append = "</color>";
                }
                else
                {
                    /*     prepend = "<color=#f7db0c>";
                         append = "</color>"; */
                }

                resultMe += word + prepend + " | " + points + append + "\n";
                myScore += points;
            }
        }
        if (resultOponent.Length > 1)
        {
            ResultOponenttextArea.text = resultOponent.Substring(0, resultOponent.Length - 1);
        }

        if (resultMe.Length > 1)
        {
            ResultPlayertextArea.text = resultMe.Substring(0, resultMe.Length - 1);
        }
        if (oponentScore < myScore)
        {
            PlayerWinName.SetActive(true);
            PlayerDefeatedName.SetActive(false);
            OponentWinName.SetActive(false);
            OponentDefeatedName.SetActive(true);
            PlayerWin.SetActive(true);
            PlayerDefeated.SetActive(false);
            OponentWin.SetActive(false);
            OponentDefeated.SetActive(true);
            MyScoreLabel.text = prependWin + "" + myScore + appendWin;
            OponentScoreLabel.text = prependLoose + "" + oponentScore + appendLoose;

            Social.ReportProgress("CgkIgYrO6MobEAIQAw", 100.0f, (bool success) => {
                // handle success or failure
            });

        }
        else if (myScore < oponentScore)
        {
            PlayerWinName.SetActive(false);
            PlayerDefeatedName.SetActive(true);
            OponentWinName.SetActive(true);
            OponentDefeatedName.SetActive(false);

            PlayerWin.SetActive(false);
            PlayerDefeated.SetActive(true);
            OponentWin.SetActive(true);
            OponentDefeated.SetActive(false);
            MyScoreLabel.text = prependLoose + "" + myScore + appendLoose;
            OponentScoreLabel.text = prependWin + "" + oponentScore + appendWin;

            Social.ReportProgress("CgkIgYrO6MobEAIQBA", 100.0f, (bool success) => {
                // handle success or failure
            });
        }
        else if (myScore == oponentScore)
        {
            PlayerWinName.SetActive(true);
            PlayerDefeatedName.SetActive(false);
            OponentWinName.SetActive(true);
            OponentDefeatedName.SetActive(false);

            PlayerWin.SetActive(true);
            PlayerDefeated.SetActive(false);
            OponentWin.SetActive(true);
            OponentDefeated.SetActive(false);

            MyScoreLabel.text = prependWin + "" + myScore + appendWin;
            OponentScoreLabel.text = prependWin + "" + oponentScore + appendWin;

            Social.ReportProgress("CgkIgYrO6MobEAIQBQ", 100.0f, (bool success) => {
                // handle success or failure
            });
        }

        //yield break;    
    }

    /*
    IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;
            if (timeLeft < 0)
            {
                GameEnded();
            }
        } 
    }
    */

    public delegate void AdviceUsedHandler();
    public event AdviceUsedHandler AdviceUsed;

    public delegate void WordEnterHandler(string word);
    public event WordEnterHandler WordEnter;

    public delegate void OponentScoreHandler(string uuid, double score);
    public event OponentScoreHandler OponentScore;
    

    public void Send()
    {       
        word = mainInputField.text.ToUpper();
        if (word.StartsWith(alpha) && CheckWord(word) && myWords.FindIndex(s => s.Contains(word.ToLower())) == -1)
        {
            textArea.text += "\n" + word.ToLower();
            myWords.Add(word.ToLower());
            mainInputField.text = "";
            WrongWord.SetActive(false);

            if (WordEnter != null) {
                WordEnter(word.ToLower());
            }
        }
        else
        {
            WrongWord.SetActive(true);
        }
    }

    //private Dictionary<string, bool> dict;
    

    public bool CheckWord(string word)
    {
        string wordLower = word.ToLower();
        return dict.CheckWord(word);
    }
    public void PrepareGame()
    {
        ResultPlayertextArea.text = "";
        ResultOponenttextArea.text = "";
        Debug.Log("NEW GAMEEEEE");
        textArea.text = "";

        Time.timeScale = 1;

        myScore = 0;
        oponentScore = 0;
        myWords.Clear();
        oponentWords.Clear();
        timeLeft = 45;
        WrongWord.SetActive(false);
        mainInputField.text = "";
        gameEnded = false;
    }
}

