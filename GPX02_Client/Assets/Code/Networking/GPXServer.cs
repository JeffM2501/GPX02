//GPX02
// Copyright 2014 Jeffery Myers

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GPXServer 
{
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

	public bool PeerConnect(NetworkPeer peer)
	{
		Debug.Log("Player Connected " + peer.OwningPlayer.guid);
		if (Players.ContainsKey(peer.OwningPlayer.guid))
		{
			Debug.Log("Duplicate player added:" + peer.OwningPlayer.guid);
			return false;
		}

		Players.Add(peer.OwningPlayer.guid, new Player(peer));
		return true;
	}

	public void PeerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Player Disconnected " + player.guid);
		if (!Players.ContainsKey(player.guid))
		{
			Debug.Log("Unknown player removed:" + player.guid);
			return;
		}

		NetworkPeer peer = Players[player.guid].Peer;
		Network.Destroy(peer.gameObject);
		Network.DestroyPlayerObjects(player);

		Players.Remove(player.guid);
	}

	public bool GetSpawnInfo(NetworkPeer peer)
	{
		peer.LastSpawn = new NetworkPeer.SpawnInfo();

		peer.LastSpawn.SpawnLocation = UnityEngine.Random.onUnitSphere * (UnityEngine.Random.value * 500);

		return true;
	}

}
