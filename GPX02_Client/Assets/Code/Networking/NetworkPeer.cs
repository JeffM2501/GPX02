//GPX02
// Copyright 2014 Jeffery Myers

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

    public ChatSystem Chat = new ChatSystem();

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

    public void SendChatMessage(int recipient, string chat)
    {
        Debug.Log("Sending chat " + chat);
        networkView.RPC("ServerReceiveChatMessage", RPCMode.Server, new object[] { recipient, chat});
    }

	// server API

	// Client RPCs
	[RPC]
	void ClientAccept(string name, int chatID, NetworkMessageInfo info)
	{
		Debug.Log("Client has accepted me");
        PlayerName = name;

        // so we know who we are
        Chat.SetMyChatID(chatID);
        Chat.KnownRecipients.Add(chatID, new ChatRecipient(chatID, name));

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

    [RPC]
    void ClientReceiveRecipient(int id, string name, NetworkMessageInfo info)
    {
        Chat.AddRecipient(id, name);
    }

    [RPC]
    void ClientReceiveChat(int from, int to, string message, NetworkMessageInfo info)
    {
        Chat.Receive(from, to, message);
    }

	// Server RPCs
	[RPC]
	void ServerHail(string magic, string name, NetworkMessageInfo info)
	{
		Debug.Log(info.sender.guid + " sent hail: " + magic);
		if (NetworkConnector.Connector.Server.PeerConnect(this))
		{
			Status = Statuses.Accepted;
			networkView.RPC("ClientAccept", info.sender, new object[]{this.PlayerName,this.Chat.MyChatID});
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

    [RPC]
    void ServerReceiveChatMessage(int recipient, string message, NetworkMessageInfo info)
    {
        NetworkConnector.Connector.Server.PeerSentChat(this, recipient, message);
    }
}