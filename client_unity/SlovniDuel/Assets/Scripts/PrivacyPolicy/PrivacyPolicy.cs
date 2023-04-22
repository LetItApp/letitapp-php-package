using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicy : MonoBehaviour
{
	public GameObject PrivacyPolicyOverlay;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("PrivacyPolicyAccepted", 0) == 0){
        	PrivacyPolicyOverlay.SetActive(true);
        }
    }

	public void Confirm()
	{
		PlayerPrefs.SetInt("PrivacyPolicyAccepted", 1);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
