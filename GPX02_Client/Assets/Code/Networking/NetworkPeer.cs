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

	public PlayerAvatar AvatarPrefab = null;

	public string PlayerName = string.Empty;
	public NetworkPlayer OwningPlayer;
	public PlayerAvatar Avatar = null;

    public int RedCards = 0;
    public static int MaxRedCards = 3;

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
	public event EventHandler PlayerSpawn;

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

    // security API
    protected void AddRedCard()
    {
		Debug.Log("Red cards added for " + PlayerName + " : " + OwningPlayer.guid);
        RedCards++;
        if (RedCards >= MaxRedCards)
        {
            Debug.Log("Max red cards hit for " + PlayerName + " : " + OwningPlayer.guid);
            Network.CloseConnection(OwningPlayer, true);
        }   
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
		Debug.Log("Server has accepted me");
        PlayerName = name;

        // so we know who we are
		Chat.SetMyChatID(chatID);
        Chat.AddRecipient(chatID, name);

		Status = Statuses.Accepted;
		if (ServerAcceptance != null)
			ServerAcceptance(this, EventArgs.Empty);
	}

	[RPC]
	void ClientSpawn(Vector3 location, NetworkMessageInfo info)
	{
		Debug.Log("Client has sent me a spawn " + location.ToString());
		Status = Statuses.Playing;
		LastSpawn.SpawnLocation = location;

		Avatar = (Network.Instantiate(AvatarPrefab, location, Quaternion.identity, 3) as GameObject).GetComponent<PlayerAvatar>();
		Avatar.gameObject.AddComponent<PlayerMover>();

		// spawn a player object
		if(PlayerSpawn != null)
			PlayerSpawn(this, EventArgs.Empty);
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
        if (info.sender != OwningPlayer || Status != Statuses.Raw || !Network.isServer)
        {
            AddRedCard();
            return;
        }

		Debug.Log(info.sender.guid + " sent hail: " + magic);
		if(magic == NetworkConnector.ConnectionMagic && NetworkConnector.Connector.Server.PeerHail(name, this))
		{
			Debug.Log("Accepted");
			Status = Statuses.Accepted;
			networkView.RPC("ClientAccept", info.sender, new object[]{this.PlayerName,this.Chat.MyChatID});

			Debug.Log("Hail status " + Status.ToString());
		}
		else
			Network.CloseConnection(info.sender,true);
	}

	[RPC]
	void ServerRequestSpawn (NetworkMessageInfo info)
	{
        if (Status == Statuses.Accepted || Status == Statuses.Dead)
        {
			Status = Statuses.Playing;
			NetworkConnector.Connector.Server.GetSpawnInfo(this);
			networkView.RPC("ClientSpawn", info.sender, this.LastSpawn.SpawnLocation);

			Debug.Log("Spawn status " + Status.ToString());
        }
		else
		{
			Debug.Log("Invalid state for spawn request: " + Status.ToString());
			AddRedCard();
			return;
		}
	}

    [RPC]
    void ServerReceiveChatMessage(int recipient, string message, NetworkMessageInfo info)
    {
        if (Status == Statuses.Raw)
        {
            AddRedCard();
            return;
        }

        NetworkConnector.Connector.Server.PeerSentChat(this, recipient, message);
    }
}