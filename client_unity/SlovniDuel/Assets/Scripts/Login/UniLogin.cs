using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Google;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class UniLogin : MonoBehaviour
{
    public string webClientId = "<your client id here>";

    public GameObject LoginPanel;
    public GameObject LoggedPanel;
    public GameObject LoginFailedLabel;
    public GameObject LoginInProgressPanel;
    public GameObject AdScript;

    public string gameName = "SLOVNI_DUEL";
    public string serverURL = "wss://ares.objedname.eu:5001";

    public UIManager m_uiManager;
    public GameConnection m_connection;

    private bool mLogged = false;
    private bool mWaitingForAuth = true;

    void Start()
    {


      //  LoginInProgressPanel.SetActive(true);
         PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        
        .RequestServerAuthCode(false)
        /*.RequestIdToken()*/
        .AddOauthScope("profile")
        .AddOauthScope("openid")
        .Build();
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.InitializeInstance(config);

        GooglePlayGames.PlayGamesPlatform.Activate(); 

        if (PlayGamesPlatform.Instance.localUser.authenticated || Social.localUser.authenticated) 
		{
			mWaitingForAuth = false;
            OnGoogleAuthenticationFinished();
        }else{
			PlayGamesPlatform.Instance.Authenticate((bool success) => {
			    Debug.Log("UNITYDEBUG:" + success);
			    if(success){
			    	OnGoogleAuthenticationFinished();
			    }else{
			    	LoginInProgressPanel.SetActive(false);
			    }	    
			    
			    
			    /*if (PlayGamesPlatform.Instance.localUser.authenticated || Social.localUser.authenticated) 
			    {
			    	OnGoogleAuthenticationFinished();
			    }else{
			    	LoginInProgressPanel.SetActive(false);
			    	
			    	PlayGamesPlatform.Instance.Authenticate((bool success2) => {
						LoginInProgressPanel.SetActive(false);

						if (PlayGamesPlatform.Instance.localUser.authenticated || Social.localUser.authenticated) 
						{
							OnGoogleAuthenticationFinished();
						}else
						{
							LoginInProgressPanel.SetActive(false);
						}
					}, true);
			    }*/
			}, true);
        }
               

        //auto login
        
        /*if(Social.localUser.authenticated){
        	mWaitingForAuth = false;
        	OnGoogleAuthenticationFinished();
        }else{
        	Social.localUser.Authenticate((bool success) => {
		        mWaitingForAuth = false;
		        if (success)
		        {
		            OnGoogleAuthenticationFinished();
		        }
		    });
        }*/
		/*
        */
    }

    void OnGoogleAuthenticationFinished()
    {
        PlayGamesPlatform.Instance.SetGravityForPopups(Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL);

        LoggedPanel.SetActive(true);
        LoginPanel.SetActive(false);

        AdScript.SetActive(true);

        //((GooglePlayGames.PlayGamesPlatform)Social.Active).SetGravityForPopups(Gravity.BOTTOM);

        Social.ReportProgress("CgkIgYrO6MobEAIQAg", 100.0f, (bool success) => {
            // handle success or failure
        });

        //string token = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
        string token = PlayGamesPlatform.Instance.GetServerAuthCode();
        
        Debug.Log("UNITYDEBUG: token" + token);
        Debug.Log("UNITYDEBUG: server auth code" + PlayGamesPlatform.Instance.GetServerAuthCode());
        
        
        Debug.Log("UNITYDEBUG: username" + Social.localUser.userName);
        
        m_uiManager.SetLabelsPlayer(Social.localUser.userName, 0, 0, 0);
        //PlayGamesPlatform.Instance.GetServerAuthCode()
        m_connection.Connect(serverURL, serverURL, "GOOGLE", Social.localUser.userName, Social.localUser.id, token);
       

        mLogged = true;
        LoginInProgressPanel.SetActive(false);
    }

    private string RandomString(int minCharAmount, int maxCharAmount)
    {
        string myString = "";
        const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789"; //add the characters you want
        int charAmount = Random.Range(minCharAmount, maxCharAmount); //set those to the minimum and maximum length of your string
        for (int i = 0; i < charAmount; i++)
        {
            myString += glyphs[Random.Range(0, glyphs.Length)];
        }
        return myString;
    }

    public void LoginDebugRequested() 
    {
        //todo comment on production
        LoggedPanel.SetActive(true);
        LoginPanel.SetActive(false);
        string nick = RandomString(8, 8);
        m_uiManager.SetLabelsPlayer(nick, 0, 0, 0);
        m_connection.Connect(serverURL, serverURL, "DEBUG", nick, RandomString(64, 64), RandomString(64, 64));        
    }

    public void LoginFacebookRequested()
    {
        //todo comment on production
        LoggedPanel.SetActive(true);
        LoginPanel.SetActive(false);
        string nick = RandomString(8, 8);
        m_uiManager.SetLabelsPlayer(nick, 0, 0, 0);
        m_connection.Connect(serverURL, serverURL, "FB", nick, RandomString(64, 64), RandomString(64, 64));       
    }
    
    public void LoginGoogleRequested()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            mWaitingForAuth = false;
            if (success)
            {
                OnGoogleAuthenticationFinished();
                Debug.Log("LOGIN OK");
            }
            else
            {
            	Debug.Log("LOGIN FAILED");
                LoginFailedLabel.SetActive(true);
                LoginInProgressPanel.SetActive(false);
            }
        });
    }

    public bool IsLogged()
    {
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
