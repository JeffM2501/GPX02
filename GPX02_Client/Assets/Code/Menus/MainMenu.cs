using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public string ServerLevel = string.Empty;
	void Start ()
    {
        if (SystemInfo.graphicsDeviceID == 0)
            StartServer();
	}

    public void StartServer()
    {
        if (ServerLevel == string.Empty)
            Application.Quit();
        else
            Application.LoadLevel(ServerLevel);
    }

    public void StartClient()
    {

    }

	void Update ()
    {
	
	}
}