using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keyboard : MonoBehaviour
{

 //   string word = null;
    int wordIndex = 0;
    string alpha;
    public InputField myInput = null;
    // Use this for initialization

    public void alphabetFunction(string alphabet)
    {
        wordIndex++;
        myInput.text = myInput.text + alphabet;

    }
    public void deleteword()
    {
        myInput.text = myInput.text.Substring
    (0, myInput.text.Length - 1);
    }
}

