using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongWord : MonoBehaviour
{
    public GameObject Wrong;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {       
    }
    IEnumerator RemoveAfterSeconds()
    {
        yield return new WaitForSeconds(1);
        Wrong.SetActive(false);
    }
}
