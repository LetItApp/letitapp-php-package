using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Share : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

	public void ShareLink(){
		new NativeShare().SetText("http://hiddenchicken.com/slovniduel").Share();
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
