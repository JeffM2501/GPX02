//GPX02
// Copyright 2014 Jeffery Myers

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class PlayerAvatar : MonoBehaviour
{
	public NetworkPeer Owner = null;

	public AudioClip SpawnSound = null;

	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		if(Network.isServer)	// verify that this avatar was made by the right person
		{
			if(NetworkConnector.Connector == null || NetworkConnector.Connector.Server == null || !NetworkConnector.Connector.Server.NewAvatar(this, info.sender))
				Network.CloseConnection(info.sender,true);
		}

		if(SpawnSound != null)
		{
			this.audio.PlayOneShot(SpawnSound);
		}
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
