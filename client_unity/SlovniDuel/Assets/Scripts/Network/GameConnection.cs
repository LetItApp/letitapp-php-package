using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

class LoginData {
    public string gameName;

    public string loginType;
    public string uuid;
    public string nick;
    public string accessToken;
}

public class PlayerFriend {
    public string uuid;
    public string nick;
    public bool isOnline;
    public double score;
}

public class ScoreboardPlayer
{
    public string uuid;
    public string nick;
    public double score;
    public bool me;
}


public class GameConnection : MonoBehaviour
{
    public UIManager m_uiManager;
    public GamePlay m_gamePlay;

    public GameObject Starttimer;

    private WebSocket m_ws;
    private bool m_connectionOpen = false;
    private bool m_logged = false;
    private bool m_inGame = false;

    private string expectedResponse = "LOGIN";
    private int m_roomID = -1;

    public delegate void GameRequestHandler(string nick, string uuid, int roomID);
    public event GameRequestHandler GameRequest;

    public delegate void OfflineHandler();
    public event OfflineHandler Offline;

    public delegate void MadePaymentHandler();
    public event MadePaymentHandler MadePayment;

    public delegate void GameEndHandler();
    public event GameEndHandler GameEnd;

    public delegate void FriendRequestResponseHandler();
    public event FriendRequestResponseHandler FriendRequestResponse;

    private NetworkAPIHttp https = new NetworkAPIHttp();

    private LoginData m_ld;

    public void UpdateProfile() {
        if (!m_connectionOpen)
            return;

        JSONNode jsonResponse = new JSONObject();
        jsonResponse["action"] = "UPDATE_PROFILE";
        m_ws.Send(jsonResponse.ToString());
    }

    public void CheckFriendOnline(string friendUUID)
    {
        if (!m_connectionOpen)
            return;

        JSONNode jsonResponse = new JSONObject();
        jsonResponse["action"] = "CHECK_FRIEND_ONLINE";
        jsonResponse["friend_id"] = friendUUID;
        m_ws.Send(jsonResponse.ToString());
    }

    private void UpdateRoom(JSONNode info)
    {
        int roomID = info["room_id"];
        UnityMainThreadDispatcher.Instance().Enqueue(() => m_roomID = roomID);

        for (int i = 0; i < info["players"].Count; i++)
        { 
           if(info["players"][i]["uuid"] == m_ld.uuid)
           {
                //its me
                string myNick = info["players"][i]["nick"];
                double myScore = info["players"][i]["score"];
                double myCoins = info["players"][i]["coins"];
                double myPremiumCoins = info["players"][i]["premium_coins"];

                UnityMainThreadDispatcher.Instance().Enqueue(() => m_uiManager.SetLabelsPlayer(myNick, myScore, myCoins, myPremiumCoins));
            }
            else {
                //Debug.Log("Oponent nick: " + info["players"][i]["nick"]);
                string oponentNick = info["players"][i]["nick"];
                double oponentScore = info["players"][i]["score"];
                string oponentUUID = info["players"][i]["uuid"];

                m_gamePlay.oponentUUID = oponentUUID;
                UnityMainThreadDispatcher.Instance().Enqueue(() => m_uiManager.SetLabelsOponent(oponentNick, oponentScore));
                
                /*if (m_uiManager.OponentName.text != null)
                {
                    Starttimer.SetActive(true);
                }
                else
                { }*/
                //Debug.Log("Oponent nick: " + info["players"][i]["nick"]);
            }
        }
    }

    public void OnAdviceUsed()
    {
        JSONNode jsonResponse = new JSONObject();
        jsonResponse["action"] = "PREMIUM_TRANSACTION";
        jsonResponse["transaction_value"] = -1;
        m_ws.Send(jsonResponse.ToString());
    }

    void RoomPayload(JSONNode json)
    {
        if (json["action"] == "ROOM_INFO")
        {
            Debug.Log("UPDATE ROOM RECEIVED");
            UpdateRoom(json["info"]);
        }

        if (json["action"] == "WORD_ENTERED")
        {
            Debug.Log("Oponent word");
            string word = json["word"];

            UnityMainThreadDispatcher.Instance().Enqueue(() => m_gamePlay.OponentWord(word));
        }

        if (json["action"] == "GAME_END")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                if (GameEnd != null)
                    GameEnd();
            });
        }
    }

    void WSMessage(string data)
    {
        Debug.Log("Received: " + data);
        
        if (expectedResponse == "LOGIN")
        {
            if (data == "OK")
            {
                m_logged = true;
                Debug.Log("LOGGED TRUE");

                UpdateProfile();
                CheckFriendRequests();

                //todo remove, zavolat po login
                //RandomGame();
            }
        }
        else if (expectedResponse == "JOIN_ROOM" && m_logged)
        {
            JSONNode json = JSON.Parse(data);

            if (json["action"] == "ROOM_INFO")
            {                
                Debug.Log("UPDATE ROOM RECEIVED");
                UpdateRoom(json["info"]); 
            }

            if (json["action"] == "GAME_STARTING")
            {
                JSONNode jsonResponse = new JSONObject();
                jsonResponse["action"] = "GAME_START_CONFIRM";
                jsonResponse["room_id"] = json["room_id"];
                m_ws.Send(jsonResponse.ToString());
            }

            if (json["action"] == "GAME_START")
            {
                expectedResponse = "GAME";
                m_inGame = true;

                int seconds = json["time"];
                float seed = json["seed"];

                long startTime = DateTimeOffset.Now.ToUnixTimeSeconds();

                UnityMainThreadDispatcher.Instance().Enqueue(() => m_uiManager.AfterOpponentConnect(seconds, startTime));
                UnityMainThreadDispatcher.Instance().Enqueue(() => m_gamePlay.GenerateLetter((int)(seed * 10000)));
            }



            /*Debug.Log("JOIN_ROOM_RANDOM");

            Debug.Log(json.ToString());

            if (json["action"] == "ROOM_INFO")
            {
                UpdateRoom(json["info"]);

                Debug.Log("UPDATE ROOM");
            }
            else
            if (json["action"] == "GAME_START")
            {
                m_inGame = true;

                m_roomID = json["roomID"];
                int seconds = json["time"];

                //todo za seconds přepnout scenu
                m_uiManager.AfterOpponentConnect(seconds);
                Debug.Log("GAMESTART");
                expectedResponse = "GAME";
            }*/
        }
        else if(expectedResponse == "GAME" && m_logged && m_inGame)
        {
            JSONNode json = JSON.Parse(data);

            /*if (json["action"] == "ROOM_INFO")
            {
                Debug.Log("UPDATE ROOM RECEIVED");
                UpdateRoom(json["info"]);
            }

            if (json["action"] == "ROOM_PAYLOAD")
            {*/
                //Debug.Log("UPDATE ROOM RECEIVED");
                //UpdateRoom(json["info"]);

                RoomPayload(json);
            //}

            Debug.Log("GAME_RECEIVED:");
        }

        try
        {
            JSONNode json = JSON.Parse(data);
            if (json["action"] == "PROFILE_UPDATE")
            {
                string myNick = json["info"]["nick"];
                double myScore = json["info"]["score"];
                double myCoins = json["info"]["coins"];
                double myPremiumCoins = json["info"]["premium_coins"];

                if (json["info"]["made_payment"] == 1) 
                {
                    if(MadePayment != null)
                        UnityMainThreadDispatcher.Instance().Enqueue(() => MadePayment());
                }

                bool transactionConfirm = (json["premium"] == true);

                UnityMainThreadDispatcher.Instance().Enqueue(() => m_uiManager.SetLabelsPlayer(myNick, myScore, myCoins, myPremiumCoins, transactionConfirm));
            }

            if (json["action"] == "FRIEND_REQUEST")
            {
                CheckFriendRequests();
            }

            if (json["action"] == "FRIEND_REQUEST_RESULT")
            {
                if(FriendRequestResponse != null)
                    UnityMainThreadDispatcher.Instance().Enqueue(() => FriendRequestResponse());
            }

            if (json["action"] == "GAME_INVITE")
            {
                string nick = json["nick"];
                string uuid = json["uuid"];
                int roomID = json["room_id"];

                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    if (GameRequest != null) {
                        GameRequest(nick, uuid, roomID);
                    }
                });
            }
        }
        catch (Exception e)
        {

        }
        

        /*else if (expectedResponse == "GAME" && m_logged && m_inGame)
        {
            JSONNode json = JSON.Parse(data);

            if (json["action"] == "ROOM_INFO")
            {
                UpdateRoom(json["info"]);                
            }
            else
            if (json["action"] == "ROOM_PAYLOAD")
            {
                JSONNode payload = json["payload"];

                if (payload["action"] == "OponentWORD")
                {
                    //todo find word in list, send points back
                }
            }
        }*/
    }

    void WSClose()
    {
        //todo reconnect
        //SceneManager.LoadScene("GUI");
        if(Offline != null)
            Offline();
    }

    void WSError()
    {
        //todo reconnect
        SceneManager.LoadScene("GUI");
    }

    void WSOpen()
    {
        m_connectionOpen = true;

        JSONNode json = new JSONObject();

        json["uuid"] = m_ld.uuid;
        json["nick"] = m_ld.nick;
        json["token"] = m_ld.accessToken;
        json["login_type"] = m_ld.loginType;
        json["action"] = "LOGIN";

        m_ws.Send(json.ToString());
    }

    public void RandomGame()
    {
        if (m_connectionOpen) {
            JSONNode json = new JSONObject();
            json["action"] = "JOIN_ROOM_RANDOM";
            expectedResponse = "JOIN_ROOM";

            Debug.Log("Sended: " + json.ToString());

            m_ws.Send(json.ToString());
        }
    }

    public void StartPrivateMatch(string otherUUID)
    {
        if (m_connectionOpen)
        {
            JSONNode json = new JSONObject();
            json["action"] = "JOIN_ROOM_PRIVATE_MATCH";

            json["expected_oponents"] = new JSONArray();
            if (otherUUID == null) {
                otherUUID = "NOT_SET";
            }
            json["expected_oponents"][0] = otherUUID;

            expectedResponse = "JOIN_ROOM";

            Debug.Log("Sended: " + json.ToString());

            m_ws.Send(json.ToString());
        }
    }

    public void Connect(string url, string gameName, string loginType, string nick, string uuid, string accessToken) 
    {
        m_ws = new WebSocket(url);

        m_ld = new LoginData();

        m_ld.accessToken = accessToken;
        m_ld.gameName = gameName;
        m_ld.loginType = loginType;
        m_ld.nick = nick;
        m_ld.uuid = uuid;

        m_ws.OnMessage += (sender, e) => WSMessage(e.Data);
        m_ws.OnOpen += (sender, e) => WSOpen();
        m_ws.OnClose += (sender, e) => WSClose();
        m_ws.OnError += (sender, e) => WSError();

        m_ws.Connect();

        GoOnline();
    }



    public void OnWordEnter(string word)
    {
        if (m_roomID != -1 && m_connectionOpen)
        {
            JSONNode json = new JSONObject();
            JSONNode jsonPayload = new JSONObject();
            JSONNode jsonPayloadPayload = new JSONObject();

            jsonPayloadPayload["action"] = "WORD_ENTERED";
            jsonPayloadPayload["word"] = word;

            jsonPayload["target"] = "ALL";
            jsonPayload["payload"] = jsonPayloadPayload;

            json["action"] = "ROOM_PAYLOAD";
            json["room_id"] = m_roomID;
            json["payload"] = jsonPayload;
            m_ws.Send(json.ToString());

            Debug.Log("SENDING: " + json.ToString());
        }
        else
        {
            Debug.Log("room ??: " + m_roomID + "" + m_connectionOpen);
        }
    }

    public void LeaveRoom()
    {
        //leave room on lobby exit
        if (m_roomID != -1 && m_connectionOpen)
        {
            JSONNode json = new JSONObject();

            json["action"] = "LEAVE_ROOM";
            json["room_id"] = m_roomID;

            m_ws.Send(json.ToString());
        }       
    }

    void Awake() 
    {
        m_uiManager.LobbyExit += new UIManager.LobbyExitHandler(LeaveRoom);

        m_uiManager.ResultExit += new UIManager.ResultExitHandler(LeaveRoom);
        m_uiManager.ResultExit += new UIManager.ResultExitHandler(UpdateProfile);



        m_gamePlay.OponentScore += new GamePlay.OponentScoreHandler(SendOponentScore);
        m_gamePlay.WordEnter += new GamePlay.WordEnterHandler(OnWordEnter);
        m_gamePlay.AdviceUsed += new GamePlay.AdviceUsedHandler(OnAdviceUsed);



        StartCoroutine("PingPong");
    }
    
    IEnumerator PingPong()
    {
        JSONNode json = new JSONObject();
        json["action"] = "PING";
        string pingMessage = json.ToString();
		while (true)
        {
            yield return new WaitForSeconds(5);
            if (m_ws != null && m_ws.ReadyState == WebSocketState.Open)
            {                              
                m_ws.Send(pingMessage);
                //Debug.Log("PING");
			}
        }	
	}

    void Update()
    {
        
    }
    
    void SendOponentScore(string oponentUUID, double result)
    {
        if (m_ws != null && m_ws.ReadyState == WebSocketState.Open)
        {
            JSONNode json = new JSONObject();
            json["action"] = "WRITE_SCORE";
            json["uuid"] = oponentUUID;
            json["score"] = result;

            m_ws.Send(json.ToString());
        }
    }

    private void OnApplicationQuit()
    {
        if (m_ws != null && m_ws.ReadyState == WebSocketState.Open) {
            LeaveRoom();
            m_ws.Close();
        }

        GoOffline();
    }

    private string serverURL = "https://gate.hiddenchicken.com";
    public string gameName = "SLOVNI_DUEL";

    public List<PlayerFriend> GetFriends()
    {
        JSONNode json = new JSONObject();
        json["action"] = "GET_FRIENDS";
        json["uuid"] = m_ld.uuid;
        json["game"] = gameName;

        string result = https.Get(serverURL + "?json=" + json.ToString());
        JSONNode arr = JSON.Parse(result);

        List<PlayerFriend> outList = new List<PlayerFriend>();

        foreach (JSONNode node in arr)
        {
            PlayerFriend f = new PlayerFriend();
            f.nick = node["Nick"];
            f.isOnline = node["IsOnline"] == 1;
            f.score = node["Score"];
            f.uuid = node["UUID"];
            outList.Add(f);
        }

        return outList;
    }



    public bool AddFriend(string nick)
    {
        JSONNode json = new JSONObject();
        json["action"] = "CREATE_FRIEND_REQUEST";
        json["uuid"] = m_ld.uuid;
        json["friend_nick"] = nick;

        string result = https.Get(serverURL + "?json=" + json.ToString());
        JSONNode arr = JSON.Parse(result);
        if (arr["STATUS"] == "OK") {
            if (m_ws != null && m_ws.ReadyState == WebSocketState.Open)
            {
                JSONNode json2 = new JSONObject();
                json2["action"] = "FRIEND_REQUEST";
                json2["uuid"] = arr["uuid"];

                m_ws.Send(json2.ToString());
            }
        }
        return arr["STATUS"] == "OK";
    }

    public List<ScoreboardPlayer> GetScoreBoard()
    {
        JSONNode json = new JSONObject();
        json["action"] = "GET_SCOREBOARD";
        json["game"] = gameName;
        string result = https.Get(serverURL + "?json=" + json.ToString());
        JSONNode arr = JSON.Parse(result);

        List<ScoreboardPlayer> scoreboard = new List<ScoreboardPlayer>();

        foreach (JSONNode node in arr) {
            ScoreboardPlayer p = new ScoreboardPlayer();
            p.nick = node["Nick"];
            p.score = node["Score"];
            p.me = (node["Nick"] == m_ld.nick);
           
            scoreboard.Add(p);
        }

        return scoreboard;
    }

    public double GetMyScore()
    {
        return 0;
    }

    public delegate void FriendRequestHandler(int id, string nick, string uuid);
    public event FriendRequestHandler FriendRequest;

    public void CheckFriendRequests()
    {
        JSONNode json = new JSONObject();
        json["action"] = "GET_FRIEND_REQUESTS";
        json["uuid"] = m_ld.uuid;
        string result = https.Get(serverURL + "?json=" + json.ToString());
                
        JSONNode arr = JSON.Parse(result);
		Debug.Log(json.ToString());
		Debug.Log(arr);
		
		if(arr["STATUS"] == "ERR"){
			return;
		}

        foreach (JSONNode node in arr)
        {
            if (FriendRequest != null)
            {
                FriendRequest(node["ID"], node["Nick"], node["UUID"]);
                
            }
        }
    }

	public void PurchaseSuccessFull(string store, string transactionID, string product, string auth)
	{	
		JSONNode json = new JSONObject();
		json["action"] = "PURCHASE";
        json["uuid"] = m_ld.uuid;
        json["store"] = store;
        json["game"] = gameName;
        json["product"] = product;
        json["transaction"] = transactionID;
        json["auth"] = auth;
        
        string result = https.Get(serverURL + "?json=" + json.ToString());
        Debug.Log(result);
	}

    public void FriendRequestResult(int id, string uuid, bool accept)
    {
        JSONNode json = new JSONObject();
        json["action"] = "FRIEND_REQUEST_RESULT";
        json["uuid"] = m_ld.uuid;
        json["friend_id"] = id;
        json["accept"] = accept;

        if (m_ws != null && m_ws.ReadyState == WebSocketState.Open)
        {
            JSONNode json2 = new JSONObject();
            json2["action"] = "FRIEND_REQUEST_RESULT";
            json2["uuid"] = uuid;

            m_ws.Send(json2.ToString());
        }


        string result = https.Get(serverURL + "?json=" + json.ToString());
        Debug.Log(result);
    }

    public void GoOnline()
    {
        JSONNode json = new JSONObject();
        json["action"] = "GO_ONLINE";
        json["uuid"] = m_ld.uuid;
        json["game"] = gameName;
        string result = https.Get(serverURL + "?json=" + json.ToString());
    }

    public void GoOffline()
    {
    	if(m_ld == null)
        	return;
        	
        JSONNode json = new JSONObject();
        json["action"] = "GO_OFFLINE";
        
        json["uuid"] = m_ld.uuid;
        json["game"] = gameName;
        
        string result = https.Get(serverURL + "?json=" + json.ToString());
    }
}
