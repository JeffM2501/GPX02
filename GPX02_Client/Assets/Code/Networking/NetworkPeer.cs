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
		networkView.RPC("ClientHail", RPCMode.Server, NetworkConnector.ConnectionMagic);
    }

	void Update ()
    {
	
	}

	// Client RPCs
	[RPC]
	public void ClientAccept(string name, NetworkMessageInfo info)
	{
		Debug.Log("Client has accepted me");
	}

	// Server RPCs
	[RPC]
	public void ClientHail(string magic, NetworkMessageInfo info)
	{
		Debug.Log(info.sender.guid + " sent hail: " + magic);
		networkView.RPC("ClientAccept", info.sender, "Player");
	}
}