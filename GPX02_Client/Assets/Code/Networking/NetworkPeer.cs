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
        if (Status != Statuses.Raw)
        {
            AddRedCard();
            return;
        }

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
        if (Status != Statuses.Accepted && Status != Statuses.Dead)
        {
            AddRedCard();
            return;
        }

		NetworkConnector.Connector.Server.GetSpawnInfo(this);
		networkView.RPC("ClientSpawn", info.sender, this.LastSpawn.SpawnLocation);
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

public class TestBehavor : MonoBehaviour
{
    public Transform playerPrefab;
    public ArrayList playerScripts = new ArrayList();
    
    void OnServerInitialized()
    {
        SpawnPlayer(Network.player);
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        SpawnPlayer(player);
    }

    void SpawnPlayer(NetworkPlayer player)
    {
        string tempPlayerString = player.ToString();
        int playerNumber = Convert.ToInt32(tempPlayerString);

        Transform newPlayerTransform = (Transform)Network.Instantiate(playerPrefab, transform.position, transform.rotation, playerNumber);
        playerScripts.Add(newPlayerTransform.GetComponent("cubeMoveAuthoritative"));

        NetworkView theNetworkView = newPlayerTransform.networkView;
        theNetworkView.RPC("SetPlayer", RPCMode.AllBuffered, player);
    }
}