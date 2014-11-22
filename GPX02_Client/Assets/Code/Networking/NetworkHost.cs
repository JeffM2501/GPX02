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
        public int Port = 2501;
        public HostData ConnectTo = new HostData();

        public int MaxConnections = 50;
    }
    public NetworkStartupOptions Options = new NetworkStartupOptions();

    public class Player
    {
        public NetworkPlayer Peer;
        public bool IsLocal = false;

        public Player()
        { 
            IsLocal = true;
        }

        public Player(NetworkPlayer netPeer)
        {
            Peer = netPeer;
            IsLocal = false;
        }
    }

    public Dictionary<string, Player> Players = new Dictionary<string, Player>();

	void Start ()
    {
        Host = this;
	}

	void Update ()
    {
	
	}

    public void Startup(GameLevel level)
    {
        Network.InitializeServer(Options.MaxConnections, Options.Port, !Network.HavePublicAddress());
        MasterServer.RegisterHost(GlobalHostID, Options.ServerName, string.Empty);
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Started");
    }

    public void AddLocalPlayer()
    {
        if (Players.ContainsKey("LocalPlayer"))
        {
            Debug.Log("Multiple Local Players Added");
            return;
        }

        Player player = new Player();
        player.IsLocal = true;
        Players.Add("LocalPlayer", player);
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

}
