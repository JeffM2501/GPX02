//GPX02
// Copyright 2014 Jeffery Myers

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GPXServer 
{
    protected int LastChatID = 2;

    public Dictionary<string, NetworkPeer> Players = new Dictionary<string, NetworkPeer>();

    public ChatSystem Chat = new ChatSystem();

	public bool PeerConnect(NetworkPeer peer)
	{
		Debug.Log("PeerConnect Player Connected " + peer.OwningPlayer.guid);
		if (Players.ContainsKey(peer.OwningPlayer.guid))
		{
			Debug.Log("Duplicate player added:" + peer.OwningPlayer.guid);
			return false;
		}
        // do a name check here...
        Players.Add(peer.OwningPlayer.guid, peer);

        // add them as a recipient to the chat system
        peer.Chat.SetMyChatID(LastChatID++);

        // tell all the clients about the recipient, including the new guy so he knows his own chat name
        SendRecipientToAll(Chat.AddRecipient(peer.Chat.MyChatID, peer.PlayerName));

        // tell just the client about the other players
        SendNewPlayerRecipients(peer);

		return true;
	}

	public bool PeerHail(string name, NetworkPeer peer)
	{
		peer.PlayerName = name;
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
		NetworkPeer peer = Players[player.guid];

        // remove this player from the list
        Players.Remove(player.guid);

        // remove them from our chat system, we don't care anymore
        // we don't tell clients to remove the ID since they may show the recipient the GUI
        Chat.RemoveRecipient(peer.Chat.MyChatID);

        // remove this player's peer, they cant' send anymore message
		Network.Destroy(peer.gameObject);

        // remove any avatars the player created, shots and stuff will live because they are server based
		Network.DestroyPlayerObjects(player);

        // tell everyone a player quit
        SentChatToAll(ChatSystem.ServerMessageSource,ChatSystem.GlobalMessageRecipient, peer.PlayerName + " has left the game");
	}

	public bool GetSpawnInfo(NetworkPeer peer)
	{
		peer.LastSpawn = new NetworkPeer.SpawnInfo();

		peer.LastSpawn.SpawnLocation = UnityEngine.Random.onUnitSphere * (UnityEngine.Random.value * 500);

		return true;
	}

    public void PeerSentChat(NetworkPeer peer, int recipient, string message)
    {
        bool valid = false;
        if (recipient == ChatSystem.GlobalMessageRecipient)
            valid = true;
        else                // we have to send to a real person
            valid = Chat.KnownRecipients.ContainsKey(recipient);

        if (valid)
            SentChatToAll(peer.Chat.MyChatID, recipient, message);
    }

    // new client functions

    protected void SendNewPlayerRecipients(NetworkPeer peer)
    {
        foreach (NetworkPeer player in Players.Values)
        {
            if (player == peer)
                continue;

            object[] args = new object[] { player.Chat.MyChatID, player.PlayerName };
            peer.networkView.RPC("ClientReceiveRecipient", peer.OwningPlayer, args);
        }
    }

    // broadcasts
    protected void SendRecipientToAll(ChatRecipient recipient)
    {
        object[] args = new object[] { recipient.ID, recipient.Name };

        foreach (NetworkPeer player in Players.Values)
        {
            player.Chat.AddRecipient(recipient.ID, recipient.Name);
            player.networkView.RPC("ClientReceiveRecipient", player.OwningPlayer, args);
        }
    }

    protected void SentChatToAll(int from, int to, string message)
    {
        object[] args = new object[] { from, to, message };

        foreach (NetworkPeer player in Players.Values)
            player.networkView.RPC("ClientReceiveChat", player.OwningPlayer, args);
    }

}
