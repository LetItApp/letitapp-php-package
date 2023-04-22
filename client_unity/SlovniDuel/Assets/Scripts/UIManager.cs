using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    public Text PlayerName;
    public Text OponentName;
    public Text[] PlayerNameResult;
    public Text[] OponentNameResult;
    public Text[] PlayerScore;
    public Text[] OponentScore;
    public Text PremiumWords;
    public Text TopScorePlayer;
    public Text TopScoreOponent;
    private int scorePlayer = 9510;
    private int scoreOponent = 5160;

    public int timetofind = 60;
    public GameObject TimerObject;
    public GameObject GamePanel;
    public GameObject LobbyPanel;
    public GameObject LoggetPanel;
    public FriendList FriendList;

    public GameObject MessagePanel;

    public Text Timer;
    public Text FindingTime;
    public GameObject WaitImageObj;
    public GameObject WaitForOponentObj;
    public int Opconnect = 0;
    public GameObject LoginScreen;

    public Shop Shop;


    public GamePlay gamePlay;

    public GameConnection m_gameConnection;

    public Text[] PlayerScores;
    public Text OponentScores;

    public Text RankInMenuVisible;

    //times
    private int beforeStartTime = 5;
    private int gamePlayCountDown = 45;

    private int currentBeforeStartTime = 0;
    private int currentGamePlayCountDown = 0;
    private int currentLobbyStartCountDown = 0;

    public GameObject LetterCooldownTimerObj;

    public GameResult GameResult;
    private bool timerRun = true;
    public StartTimer BeforeStartTimer;
    private long startTime;

    public AdScript AdScript;

    public Text[] PremiumWordsLabels;

    // Start is called before the first frame update
    void Start()
    {
        PrepareLobby();

        Shop.InitShop();

        if (LoginScreen.activeInHierarchy==true)
        {
            LoggetPanel.SetActive(false);
        }
        else if (LoginScreen.activeInHierarchy==false)
        {
            LoggetPanel.SetActive(true);
        }

        //friend list
        m_gameConnection.FriendRequest += new GameConnection.FriendRequestHandler(FriendList.OnFriendRequest);
        m_gameConnection.GameRequest += new GameConnection.GameRequestHandler(FriendList.OnGameRequest);
        m_gameConnection.MadePayment += new GameConnection.MadePaymentHandler(() => {
            AdScript.HideAds();
        });

        m_gameConnection.GameEnd += new GameConnection.GameEndHandler(() => {
            Debug.Log("end");
            gamePlay.GameEnded();
        });

        m_gameConnection.Offline += new GameConnection.OfflineHandler(() => {
            MessagePanel.GetComponent<MessagePanel>().ShowMessage(MessageType.ERROR, "Bez připojení", "Prosím zkontrolujte připojení k internetu a restartujte hru", false);
        });

        //m_gameConnection.CheckFriendRequests();
    }

    public delegate void ResultExitHandler();
    public event ResultExitHandler ResultExit;
    
    public void OnResultExit()
    {
        if(ResultExit != null)
            ResultExit();
    }

    private void Awake()
    {
        GameResult.ResultExit += OnResultExit;
    }

    // Update is called once per frame
    void Update()
    {
        Timer.text = ("" + currentLobbyStartCountDown);
        FindingTime.text = (""+timetofind);
    }

    public void SetLabelsPlayer(string nick, double score, double coins, double premium_coins, bool transactionConfirm = false)
    {
        PlayerName.text = nick;
        TopScorePlayer.text = score.ToString() + " skóre";
        RankInMenuVisible.text = score.ToString();

        if (transactionConfirm)
        {
            gamePlay.lastTransactionConfirmed = true;
        }

        gamePlay.PremiumWordsLeft = premium_coins;

        foreach (Text T in PlayerNameResult)
        {
            T.text = nick;
        }

        foreach (Text T in PlayerScores)
        {
            T.text = score.ToString() + " skóre";
        }

        foreach (Text T in PremiumWordsLabels)
        {
            T.text = premium_coins + "x";
        }
    }
    
    public void SetLabelsOponent(string nick, double score)
    {
        OponentName.text = nick;

        OponentName.SetLayoutDirty(); //black magic?

         TopScoreOponent.text = score.ToString() + " skóre";
        foreach (Text A in OponentNameResult)
        {
            A.text = nick;
        }
    }

    public void AfterOpponentConnect(int seconds, long startSecond)
    {
        Opconnect += 1;

        currentLobbyStartCountDown = seconds; //10s jde ze serveru
        currentBeforeStartTime = beforeStartTime;
        currentGamePlayCountDown = gamePlayCountDown;

        timerRun = true;
        startTime = startSecond;
        
        StartCoroutine(StartTimer());

        WaitImageObj.SetActive(false);
        WaitForOponentObj.SetActive(false);
        TimerObject.SetActive(true);
    }

    public void LoadGame()
    {
        GamePanel.SetActive(true);
        LobbyPanel.SetActive(false);        
    }

   IEnumerator StartTimer()
    {
        

        while (timerRun)
        {
            long unixSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

            long elapsedSeconds = unixSeconds - startTime;
            startTime = unixSeconds;

            //to not hunt CPU
            yield return new WaitForSeconds(0.001f);

            for (int i = 0; i < elapsedSeconds; i++)
            {
                currentLobbyStartCountDown--;

                if (currentLobbyStartCountDown == 0)
                {
                    LoadGame();
                    PrepareLobby();
                    BeforeStartTimer.PrepareTimer();
                }

                if (currentLobbyStartCountDown < 0)
                {
                    currentBeforeStartTime--;
                    if (currentBeforeStartTime >= 0)
                    {
                        BeforeStartTimer.SetTime(currentBeforeStartTime);
                    }

                    if (currentBeforeStartTime < 0)
                    {
                        currentGamePlayCountDown--;

                        gamePlay.SetTimeLeft(currentGamePlayCountDown);

                        if (currentGamePlayCountDown == 0)
                        {
                            //gamePlay.GameEnded(); WAWA

                            timerRun = false;
                            yield break;
                        }
                    }




                }
            }

            

            

            /*gamePlayCountDown--;

            if (gamePlayCountDown < 0)
            {
                LoadGame();
                run = false;
                timeLeft = 15;
                PrepareLobby();
                yield break;
            }*/
        }
    }

    public delegate void LobbyExitHandler();
    public event LobbyExitHandler LobbyExit;
    


    IEnumerator TryToFindOponent()
    {
        bool run = true;
        while (run)
        {
            yield return new WaitForSeconds(1);
            timetofind--;
            if (timetofind < 0 && WaitImageObj.activeInHierarchy==true)
            {
                OponentNotFound();                
                StopCoroutine("TryToFindOponent");
                FindingTimeReset();

                if (LobbyExit != null)
                {
                    LobbyExit();
                }
            }
            else if (Opconnect>0)
            {
               // StopCoroutine("TryToFindOponent");
                WaitForOponentObj.SetActive(false);
                Opconnect = 0;
                run = false;
                yield break;
            }
        }
    }

    public void OponentNotFound()
    {
        LoggetPanel.SetActive(true);
        LobbyPanel.SetActive(false);
    }

    public void StartLobbyFoundTimer()
    {
        StartCoroutine("TryToFindOponent");
    }

    public void StartLobbyFriendTimer(string nick, double score)
    {
        SetLabelsOponent(nick, score);
        StartCoroutine("TryToFindOponent");
    }


    public void StartPrivateMatch(string friendUUID, string nick, double friendScore) {
        //m_gameConnection.StartPrivateMatch(friendUUID);

        GamePanel.SetActive(false);
        LoggetPanel.SetActive(false);
        LobbyPanel.SetActive(true);

        StartLobbyFriendTimer(nick, friendScore);
    }


    public void FindingTimeReset()
    {
        timetofind = 60;
    }

    public void PrepareLobby()
    {
        Debug.Log("I dont want to be a queen of ashes. Hahahahhahaa");
        WaitImageObj.SetActive(true);
        WaitForOponentObj.SetActive(true);
        TimerObject.SetActive(false);
        LetterCooldownTimerObj.SetActive(true);
        OponentName.text = "Jméno oponenta";
        timetofind = 60;
        Opconnect = 0;
    }
}
