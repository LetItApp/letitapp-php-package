using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FriendList : MonoBehaviour
{
    public GameConnection m_gameConnection;
    public GameObject FriendPrefab;
    public GameObject Content;

    public GameObject MessagePanel;
    public GameObject ConfirmPanel;
    public UIManager UIManager;

    private string NickEntered;
    public InputField NickInputField;

    public void AddFriendButton()
    {
        NickEntered = NickInputField.text.ToString();
        if (m_gameConnection.AddFriend(NickEntered))
        {
            MessagePanel.GetComponent<MessagePanel>().ShowMessage(MessageType.SUCCESS, "Úspěch", "Pozvánka byla odeslána", true);
            NickInputField.text = "";
        }
        else {
            MessagePanel.GetComponent<MessagePanel>().ShowMessage(MessageType.ERROR, "Chyba", "Úživatel s tímto nickem nebyl nalezen.", true);
        }
        
    }

    public void OnFriendRequest(int ID, string nick, string uuid)
    {
        ConfirmPanel.GetComponent<ConfirmBox>().Show("Pozvánka", "Uživatel " + nick + " si přeje být vašim přítelen", 
            ()=> {
                m_gameConnection.FriendRequestResult(ID, uuid, true);
                OnEnable();
            }, 
            () => {
                m_gameConnection.FriendRequestResult(ID, uuid, false);
            } 
        );
    }

    public void OnGameRequest(string nick, string uuid, int roomID)
    {
        ConfirmPanel.GetComponent<ConfirmBox>().Show("Pozvánka", "Uživatel " + nick + " si přeje s Vámi hrát",
            () => {
                m_gameConnection.StartPrivateMatch(uuid);
                UIManager.StartPrivateMatch(uuid, nick, roomID);
            },
            () => {

            }
        );
    }

    void OnEnable()
    {
        RefreshFriendList();

        m_gameConnection.FriendRequestResponse += new GameConnection.FriendRequestResponseHandler(RefreshFriendList);
    }

    public void RefreshFriendList()
    {
        List<PlayerFriend> friendlist = m_gameConnection.GetFriends();

        foreach (Transform child in Content.transform)
        {
            if (child.name != "FriendPrefab")
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        foreach (PlayerFriend p in friendlist)
        {
            GameObject item = Instantiate(FriendPrefab, Content.transform, true);
            item.SetActive(true);

            item.GetComponent<PrivateMatchStarter>().friendUUID = p.uuid;
            item.GetComponent<PrivateMatchStarter>().nick = p.nick;
            item.GetComponent<PrivateMatchStarter>().friendScore = p.score;

            foreach (Transform child in item.transform)
            {
                if (child.name == "FriendScore")
                {
                    child.GetComponent<Text>().text = p.score + "";
                }
                else
                if (child.name == "FriendName")
                {
                    child.GetComponent<Text>().text = p.nick;
                }
                else
                if (child.name == "FriendOffline" || child.name == "FriendOnline" || child.name == "Block")
                {
                    if (child.name == "FriendOnline" && p.isOnline)
                    {
                        child.gameObject.SetActive(true);
                    }
                    if (child.name == "FriendOnline" && !p.isOnline)
                    {
                        child.gameObject.SetActive(false);
                    }
                    if (child.name == "FriendOffline" && p.isOnline)
                    {
                        child.gameObject.SetActive(false);
                    }
                    if (child.name == "FriendOffline" && !p.isOnline)
                    {
                        child.gameObject.SetActive(true);
                    }
                    if (p.isOnline)
                    {
                        item.GetComponent<Button>().interactable = true;
                    }
                    else
                    {
                        item.GetComponent<Button>().interactable = false;
                    }
                }
            }          
        }
    }

}
