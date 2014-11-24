using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public string ServerLevel = string.Empty;
	public string ClientLevel = string.Empty;

	public ListBox ServerList = null;

	void Start ()
    {
        if (SystemInfo.graphicsDeviceID == 0)
            StartServer();

		MasterServer.RequestHostList(NetworkHost.GlobalHostID);
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
		{
			if (ServerList != null)
			{
				ServerList.ClearItems();
				foreach (HostData host in MasterServer.PollHostList())
					ServerList.AddItem(host.gameName, host);
			}
		}
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
		Application.LoadLevel(ClientLevel);
    }

	void Update ()
    {
	
	}
}