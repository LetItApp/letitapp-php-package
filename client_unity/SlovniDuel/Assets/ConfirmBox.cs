using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmBox : MonoBehaviour
{
    public Text MessageTitle;
    public Text MessageLabel;

    public GameObject ConfirmButton;
    public GameObject DenyButton;

    public GameObject FightInviteIcon;
    public GameObject FriendRequestIcon;

    Action mOnConfirm;
    Action mOnDeny;
        
    public void ConfirmClicked()
    {
        mOnConfirm();
    }

    public void DenyClicked()
    {
        mOnDeny();
    }

    public void Show(string title, string message, Action onConfirm, Action onDeny ) {
        mOnConfirm = onConfirm;
        mOnDeny = onDeny;

        MessageTitle.text = title;
        MessageLabel.text = message;

        gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
