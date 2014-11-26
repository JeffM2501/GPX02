using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatLineGUI : MonoBehaviour 
{
	public Text ChatLine = null;
	public Image AvatarImage = null;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void Setup(Sprite avatar, string text)
	{
		if (ChatLine != null)
			ChatLine.text = text;

		if (AvatarImage != null)
			AvatarImage.sprite = avatar;
	}
}
