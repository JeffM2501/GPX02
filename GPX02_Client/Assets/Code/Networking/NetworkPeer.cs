using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class NetworkPeer : MonoBehaviour
{
	protected bool IsLocalPeer = false;

	public NetworkPlayer OwningPlayer;

	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		OwningPlayer = info.sender;
		if (Network.isServer)
			NetworkConnector.Connector.Server.PeerConnect(this);
	}

    // globals
    public void SetLocalPeer()
    {
		IsLocalPeer = true;
    }

	void Update ()
    {
	
	}
}