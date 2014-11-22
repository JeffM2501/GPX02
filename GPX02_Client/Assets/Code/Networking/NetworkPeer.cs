using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class NetworkPeer : MonoBehaviour
{
    // globals
    public static HostData TargetServer = new HostData();

    protected bool IsLocal = false;

	void Start ()
    {
       
	}

	public void ConnectLocal()
	{
		IsLocal = NetworkHost.Host != null;

		if (!IsLocal)
		{
			Network.Connect(TargetServer);
			Debug.Log("Connecting to " + TargetServer.ToString());
		}
		else
		{
			NetworkHost.Host.AddLocalPlayer(this);
		}
	}

    void OnConnectedToServer()
    {
        Debug.Log("Host connection established");
        StartClient();

		if (IsLocal)
			NetworkHost.Host.ClientHail(NetworkHost.ConnectionMagic,)
        this.networkView.RPC("ClientHail",RPCMode.Server,NetworkHost.ConnectionMagic);
    }

    public void StartClient()
    {

    }

	void Update ()
    {
	
	}
}