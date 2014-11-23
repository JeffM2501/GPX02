using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class NetworkPeer : MonoBehaviour
{
    // globals
    public static HostData TargetServer = null;

	void Start ()
    {
        if (TargetServer == null && Application.isEditor)
            Network.Connect("localhost", NetworkHost.DefaultPort);
        else
            Network.Connect(TargetServer);
        Debug.Log("Connecting to host");
	}

    void OnConnectedToServer()
    {
        Debug.Log("Host connection established ");
        StartClient();
        networkView.RPC("ClientHail",RPCMode.Server,NetworkHost.ConnectionMagic);
    }

    public void StartClient()
    {

    }

	void Update ()
    {
	
	}
}