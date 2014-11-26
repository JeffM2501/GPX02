//GPX02
// Copyright 2014 Jeffery Myers

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ChatMessage
{
	public int RecipientID = -1;
    public int SourceID = -1;
    public string MessageText = string.Empty;

    public ChatRecipient From = null;

    public enum Types
    {
        General = 0,
        Server,
        Direct,
    }

    public Types MessageType = Types.General;
}

public class ChatRecipient
{
    public int ID = -1;
    public string Name = string.Empty;

    public object Tag = null;

    public ChatRecipient (int id, string name)
    {
        ID = id;
        Name = name;
    }
}

public class ChatSystem
{
    public int MyChatID = -1;

    public static int GlobalMessageRecipient = 0;
    public static int ServerMessageSource = 1;

    public List<ChatMessage> Messages = new List<ChatMessage>();
    public Dictionary<int, ChatRecipient> KnownRecipients = new Dictionary<int, ChatRecipient>();

    public class RecipientEventArgs : EventArgs
    {
        public ChatRecipient Recipient = null;

        public RecipientEventArgs(ChatRecipient r)
        {
            Recipient = r;
        }
    }

    public event EventHandler LocalChatIDSet;
    public event EventHandler TextMessageReceived;
    public event EventHandler<RecipientEventArgs> NewRecipient;   

    public void SetMyChatID(int id)
    {
        MyChatID = id;

        if (LocalChatIDSet != null)
            LocalChatIDSet(this, EventArgs.Empty);
    }

    public void Receive(int from, int to, string chat)
    {
        ChatMessage msg = new ChatMessage();
        if (to == GlobalMessageRecipient)
            msg.MessageType = ChatMessage.Types.General;
        else if (from == ServerMessageSource)
            msg.MessageType = ChatMessage.Types.Server;
        else if (KnownRecipients.ContainsKey(from))
            msg.MessageType = ChatMessage.Types.Direct;
        else
            Debug.Log("Unknown message recipient " + from.ToString());
    
        if (KnownRecipients.ContainsKey(from))
            msg.From = KnownRecipients[from];

        msg.RecipientID = to;
        msg.SourceID = from;
        msg.MessageText = chat;

        Messages.Add(msg);

        if (TextMessageReceived != null)
            TextMessageReceived(this, EventArgs.Empty);
    }

    public ChatRecipient AddRecipient(int id, string name)
    {
        if (KnownRecipients.ContainsKey(id))
            KnownRecipients[id].Name = name;
        else
            KnownRecipients.Add(id, new ChatRecipient(id, name));

        ChatRecipient r = KnownRecipients[id];

        if (NewRecipient != null)
            NewRecipient(this, new RecipientEventArgs(r));

        return r;
    }

    public void RemoveRecipient(int id)
    {
        if (KnownRecipients.ContainsKey(id))
            KnownRecipients.Remove(id);
    }

    public void PruneMessageCount(int count)
    {
        if (Messages.Count <= count)
            return;

        Messages.RemoveRange(0, Messages.Count - count);
    }
}