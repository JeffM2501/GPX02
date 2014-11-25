using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public string ServerLevel = string.Empty;
	public string ClientLevel = string.Empty;

    public Button ClientStartButton = null;

	public ListBox ServerList = null;

	void Start ()
    {
        if (SystemInfo.graphicsDeviceID == 0)
            StartServer();

        if (ClientStartButton != null)
            ClientStartButton.interactable = false;
       
		MasterServer.RequestHostList(NetworkHost.GlobalHostID);

        if (ServerList != null)
            ServerList.SelectionChanged += ServerList_SelectionChanged;
	}

    void ServerList_SelectionChanged(object sender, EventArgs e)
    {
        if (ClientStartButton != null)
            ClientStartButton.interactable = ServerList.GetSelectedItem() != null;

        NetworkPeer.TargetServer = ServerList.GetSelectedItemTag() as HostData;
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