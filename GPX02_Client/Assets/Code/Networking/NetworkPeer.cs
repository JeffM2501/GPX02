using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class NetworkPeer : MonoBehaviour
{
	protected bool IsLocalPeer = false;

	public string PlayerName = string.Empty;
	public NetworkPlayer OwningPlayer;

	public class SpawnInfo
	{
		public Vector3 SpawnLocation = Vector3.zero;
	}
	public SpawnInfo LastSpawn = null;

	public enum Statuses
	{
		Raw,
		Accepted,
		Playing,
		Dead,
	}

	public Statuses Status = Statuses.Raw;

	public event EventHandler ServerAcceptance;

	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		this.name = "NetworkPeer:" + info.sender.guid;
		OwningPlayer = info.sender;
		if (Network.isServer)
			NetworkConnector.Connector.Server.PeerConnect(this);
	}

    // globals
    public void SetLocalPeer(string name)
    {
		PlayerName = name;
		IsLocalPeer = true;
		networkView.RPC("ServerHail", RPCMode.Server, new object[]{NetworkConnector.ConnectionMagic as object, name as object});
    }

	void Update ()
    {
		
	}

	// client API

	public void RequestSpawn()
	{
		Debug.Log("Sending spawn request");
		networkView.RPC("ServerRequestSpawn", RPCMode.Server, null);
	}

	// server API

	// Client RPCs
	[RPC]
	void ClientAccept(string name, NetworkMessageInfo info)
	{
		Debug.Log("Client has accepted me");
		Status = Statuses.Accepted;
		if (ServerAcceptance != null)
			ServerAcceptance(this, EventArgs.Empty);
	}

	[RPC]
	void ClientSpawn(Vector3 location, NetworkMessageInfo info)
	{
		Debug.Log("Client has sent me a spawn " + location.ToString());
		Status = Statuses.Playing;
		
		// spawn a player object
	}

	// Server RPCs
	[RPC]
	void ServerHail(string magic, string name, NetworkMessageInfo info)
	{
		Debug.Log(info.sender.guid + " sent hail: " + magic);
		if (NetworkConnector.Connector.Server.PeerConnect(this))
		{
			Status = Statuses.Accepted;
			networkView.RPC("ClientAccept", info.sender, this.PlayerName);
		}
		else
			Network.CloseConnection(info.sender,true);
	}

	[RPC]
	void ServerRequestSpawn (NetworkMessageInfo info)
	{
		NetworkConnector.Connector.Server.GetSpawnInfo(this);
		networkView.RPC("ClientSpawn", info.sender, this.LastSpawn.SpawnLocation);
	}
}