using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MessageType {
    SUCCESS,
    INFO, 
    ERROR
};

public class MessagePanel : MonoBehaviour
{
    public Text MessageTitle;
    public Text MessageLabel;
    public GameObject ButtonClose;
    public GameObject Panel;

    public void ShowMessage(MessageType type, string title, string message, bool canBeClosed)
    {
        MessageTitle.text = title;
        MessageLabel.text = message;

        gameObject.SetActive(true);
		Panel.SetActive(true);

        switch (type)
        {
            case MessageType.INFO:
                //todo
                break;

            case MessageType.SUCCESS:

                break;

            case MessageType.ERROR:

                break;

            default:
                break;
        }

        if (canBeClosed)
        {
            ButtonClose.SetActive(true);
        }
        else
        {
            ButtonClose.SetActive(false);
        }
    }
}
