//GPX02
// Copyright 2014 Jeffery Myers

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class NetworkHost : MonoBehaviour
{
    // constants
    public static string GlobalHostID = "Inverted.Polarity.GPX02.DEV";
    public static string ConnectionMagic = "GP_Proto.1";
    public static int DefaultPort = 2501;

    // singletons
    public static NetworkHost Host = null;

    // host members
    public class NetworkStartupOptions
    {
        public string ServerName = "DebugServer";
		public int Port = DefaultPort;
        public HostData ConnectTo = new HostData();

        public int MaxConnections = 50;
    }
    public NetworkStartupOptions Options = new NetworkStartupOptions();

    public class Player
    {
        public NetworkPlayer Socket;
		public NetworkPeer Peer;		
        public bool IsLocal = false;

        public Player(NetworkPeer peer)
        { 
            IsLocal = true;
			Peer = peer;
        }

        public Player(NetworkPlayer netPeer)
        {
			Socket = netPeer;
            IsLocal = false;
        }
    }

    public Dictionary<string, Player> Players = new Dictionary<string, Player>();

	void Awake()
	{
		Host = this;
	}

	void Start ()
    {
        Host = this;
	}

	void Update ()
    {
	
	}

    public void Startup(GameLevel level)
    {
        ConnectionTesterStatus netStatus = Network.TestConnection(false);

        bool validNetwork = netStatus != ConnectionTesterStatus.Error;

		Debug.Log("Network Host Startup");

        if (Network.HavePublicAddress())
            Debug.Log("has a public address");

        Debug.Log(netStatus);

        Network.InitializeServer(Options.MaxConnections, Options.Port, validNetwork && !Network.HavePublicAddress());

        if (validNetwork)
			MasterServer.RegisterHost(GlobalHostID, Options.ServerName, string.Empty);
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Started");
    }

    public void AddLocalPlayer(NetworkPeer peer)
    {
        if (Players.ContainsKey("LocalPlayer"))
        {
            Debug.Log("xMultiple Local Players Added");
            return;
        }

        Player player = new Player(peer);
        player.IsLocal = true;
        Players.Add("LocalPlayer", player);
		peer.StartClient();
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player Connected " + player.guid);
        if (Players.ContainsKey(player.guid))
        {
            Debug.Log("Duplicate player added:" + player.guid);
            return;
        }

        Players.Add(player.guid, new Player(player));
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Player Disconnected " + player.guid);
        if (!Players.ContainsKey(player.guid))
        {
            Debug.Log("Unknown player removed:" + player.guid);
            return;
        }

        Players.Remove(player.guid);
    }

	// client RPCs
	[RPC]
	public void ClientHail (string magic, NetworkMessageInfo info)
	{
		Debug.Log(info.sender.guid + " sent hail: " + magic);
	}

}
