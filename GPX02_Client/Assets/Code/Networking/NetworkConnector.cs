﻿//GPX02
// Copyright 2014 Jeffery Myers

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class NetworkConnector : MonoBehaviour
{
    // constants
    public static string GlobalHostID = "Inverted.Polarity.GPX02.DEV";
    public static string ConnectionMagic = "GP_Proto.1";
    public static int DefaultPort = 2501;

	public static HostData TargetServer = null;

	public static bool StartAsServer = false;

	public NetworkPeer PeerPrefab = null;

    public NetworkPeer LocalPeer = null;    // the local player so that GUI and input can access it
	public GPXServer Server = null;         // the server so the GUI can access it

    public event EventHandler LocalClientConnected; // we are a client and we connected, sender will be local client NetworkPeer
    public event EventHandler ServerStarted;        // we are a server and we are listening, sender will be GPXServer class

    // singletons
	public static NetworkConnector Connector = null;

    // host members
    public class NetworkStartupOptions
    {
        public string ServerName = "DebugServer";
		public int Port = DefaultPort;
        public HostData ConnectTo = new HostData();

        public int MaxConnections = 50;
    }
    public NetworkStartupOptions Options = new NetworkStartupOptions();

	void Awake()
	{
		Connector = this;
	}

	void Start ()
    {
		Connector = this;

		if(GameObject.FindGameObjectWithTag("Server") != null)
			StartAsServer = true;

		if (StartAsServer)
		{
			Server = new GPXServer();
		}
		else
		{
			if (TargetServer == null && Application.isEditor)
				Network.Connect("localhost", DefaultPort);
			else
				Network.Connect(TargetServer);
			Debug.Log("Connecting to host");
		}
	}

	void Update ()
    {
	
	}

    public void Startup(GameLevel level)
    {
		if (StartAsServer)
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

            if (ServerStarted != null)
                ServerStarted(Server, EventArgs.Empty);
		}
		else
		{
		
		}
    }

	public void Shutdown()
	{
		if(Network.isServer && Server != null)
			Server.Shutdown();

		Network.Disconnect();
	}

	// client messages
	void OnConnectedToServer()
	{
		Debug.Log("Host connection established ");
		if (PeerPrefab == null)
		{
			Debug.Log("No Peer Prefab");
			Application.Quit();
		}

		GameObject obj = Network.Instantiate(PeerPrefab.gameObject, Vector3.zero, Quaternion.identity, 0) as GameObject;
		if (obj == null)
		{
			Debug.Log("Peer Instantiate Failed");
			Application.Quit();
		}

        LocalPeer = obj.GetComponent<NetworkPeer>();
        if (LocalPeer == null)
		{
			Debug.Log("Peer Prefab component failure");
			Application.Quit();
		}

        LocalPeer.SetLocalPeer("PlayerName");

        if (LocalClientConnected != null)
            LocalClientConnected(LocalPeer, EventArgs.Empty);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Debug.Log("Client Disconnected " + info.ToString());

		Application.Quit();
	}

	// serer messages
    void OnServerInitialized()
    {
        Debug.Log("Server Started");
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
		Debug.Log("Player Connected " + player.guid);
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
		Debug.Log("Player Disconnected " + player.guid);
		Server.PeerDisconnected(player);
    }
}
