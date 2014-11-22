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
        IsLocal = NetworkHost.Host != null;

        if (!IsLocal)
        {
            Network.Connect(TargetServer);
            Debug.Log("Connecting to " + TargetServer.ToString());
        }
	}

    void OnConnectedToServer()
    {
        Debug.Log("Host connection established");
        StartClient();

        this.networkView.RPC("ClientHail",RPCMode.Server,NetworkHost.ConnectionMagic);
    }

    public void StartClient()
    {

    }

	void Update ()
    {
	
	}
}