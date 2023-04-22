using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PrivateMatchStarter : MonoBehaviour
{
    public string nick;
    public string friendUUID;
    public double friendScore;

    public GameObject ConfirmPanel;
    public GameConnection m_gameConnection;
    public GameObject LoggedPanel;
    public GameObject LobbyPanel;
    public UIManager UIManager;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { OnMouseDown(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        ConfirmPanel.GetComponent<ConfirmBox>().Show("Privátní hra", "Přejete si založit privátní hru s " + nick + "?",
            () => {
                m_gameConnection.StartPrivateMatch(friendUUID);
                UIManager.StartPrivateMatch(friendUUID, nick, friendScore);

                //LoggedPanel.SetActive(false);
                //LobbyPanel.SetActive(true);
                //UIManager.StartLobbyFriendTimer(nick, friendScore);
            },
            () => {

            }
        );
    }
}
