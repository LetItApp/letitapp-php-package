using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class Dictionary : MonoBehaviour
{
    private Dictionary<string, bool> dict;
    public TextAsset words;
    private Thread thrLoad;
    private List<string> tmpList;

    private Dictionary<char, Dictionary<string, bool>> accDict;

    public void Start() {
        
    }

    public void LoadDict(byte[] bytes)
    {
        Dictionary<string, bool> tmpDict = new Dictionary<string, bool>();
        string line;
        using (var streamReader = new StreamReader(new MemoryStream(bytes)))
        {
            while ((line = streamReader.ReadLine()) != null)
            {
                string word = line.ToLower();
                tmpDict[word] = true;
                if (!accDict.ContainsKey(word[0])) {
                    accDict[word[0]] = new Dictionary<string, bool>();
                }
                accDict[word[0]][word] = true;
            }
        }
        Debug.Log("Dict loaded");
        dict = tmpDict;
    }

    void Awake()
    {
        byte[] bytes = words.bytes;

        tmpList = new List<string>();
        accDict = new Dictionary<char, Dictionary<string, bool>>();

        thrLoad = new Thread(() => LoadDict(bytes));
        thrLoad.Start();
    }

    public bool CheckWord(string word){
        if (dict == null)
            return false;

        return dict.ContainsKey(word.ToLower());
    }

    public string GetAdvise(List<string> usedWords, string letter)
    {
        if (dict == null)
            return null;

        tmpList.Clear();

        foreach (string w in accDict[letter.ToLower()[0]].Keys)
        {
            if (!usedWords.Contains(w))
            {
                tmpList.Add(w);
            }

            /* if (w.StartsWith(letter))
             {

             }*/
        }

        int index = Random.Range(0, tmpList.Count);
        if (tmpList.Count > 0) {
            return tmpList[index];
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
