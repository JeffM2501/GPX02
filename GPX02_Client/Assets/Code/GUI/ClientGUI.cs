//GPX02
// Copyright 2014 Jeffery Myers

using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ClientGUI : MonoBehaviour
{
	public RectTransform SpawnGroup = null;
	public RectTransform EscapeMenuGroup = null;

	public Text ConnectionText = null;

	public string ExitLevel = string.Empty;
	public NetworkPeer ClientPeer = null;

	protected bool SpawnIsUp = false;
	protected bool EscapeIsUp = false;

	protected bool CanSpawn = false;

	// Use this for initialization
	void Start () 
	{
		NetworkConnector.Connector.LocalClientConnected += Connector_LocalClientConnected;

		if(ConnectionText != null)
			ConnectionText.gameObject.SetActive(true);

		if(EscapeMenuGroup != null)
			EscapeMenuGroup.gameObject.SetActive(false);

		if (SpawnGroup != null)
		{
			SpawnGroup.gameObject.SetActive(false);
		}
	}

	public void Exit()
	{
		NetworkConnector.Connector.Shutdown();
		Application.Quit();
	}

	public void Logout()
	{
		NetworkConnector.Connector.Shutdown();
		Application.LoadLevel(ExitLevel);
	}

	public void Spawn()
	{
		if(EscapeIsUp || !CanSpawn && NetworkConnector.Connector.LocalPeer != null)
			return;

		// do spawn
		NetworkConnector.Connector.LocalPeer.RequestSpawn();

	}

	void Connector_LocalClientConnected(object sender, System.EventArgs e)
	{
		if(ConnectionText != null)
			ConnectionText.gameObject.SetActive(false);
		NetworkConnector.Connector.LocalPeer.ServerAcceptance += LocalPeer_ServerAcceptance;
		NetworkConnector.Connector.LocalPeer.PlayerSpawn += LocalPeer_PlayerSpawn;
	}

	void LocalPeer_PlayerSpawn(object sender, System.EventArgs e)
	{
		CanSpawn = false;
		if(!EscapeIsUp)
		{
			SpawnIsUp = false;
			SpawnGroup.gameObject.SetActive(false);
		}
	}

	void LocalPeer_ServerAcceptance(object sender, System.EventArgs e)
	{
		CanSpawn = true;
		if (!EscapeIsUp)
		{
			SpawnIsUp = true;
			SpawnGroup.gameObject.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (EscapeIsUp)
			{
				EscapeMenuGroup.gameObject.SetActive(false);
				EscapeIsUp = false;

				if (CanSpawn)
					SpawnGroup.gameObject.SetActive(true);
			}
			else
			{
				SpawnGroup.gameObject.SetActive(false);
				EscapeMenuGroup.gameObject.SetActive(true);
			}
		}
	}
}
