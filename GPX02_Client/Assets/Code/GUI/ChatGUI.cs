using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatGUI : MonoBehaviour
{
	public ChatLineGUI ChatItemPrefab = null;
	public RectTransform ChatFrame = null;

	public InputField ChatLine = null;

	public ChatSystem Chat = null;

	public int MaxLines = 50;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void SetChat(ChatSystem chat)
	{
		Chat = chat;

		Chat.TextMessageReceived += Chat_TextMessageReceived;
	}

	void Chat_TextMessageReceived(object sender, System.EventArgs e)
	{
		if (ChatFrame == null || ChatItemPrefab == null)
			return;

		Chat.PruneMessageCount(MaxLines);

		
		for(int i = Chat.Messages.Count-1; i >= 0; i--)
		{
			if (Chat.Messages[i].Tag != null)
				return;

			GameObject obj = GameObject.Instantiate(ChatItemPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;

			if (obj == null)
				return;
			ChatLineGUI item = obj.GetComponent<ChatLineGUI>();
			item.Setup(null, Chat.Messages[i].MessageText);
			item.gameObject.transform.SetParent(ChatFrame.transform, true);
			Chat.Messages[i].Tag = item;
		}
	}
}
