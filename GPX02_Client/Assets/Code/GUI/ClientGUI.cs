using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientGUI : MonoBehaviour
{
	public Button MenuButton = null;
	public string ExitLevel = string.Empty;
	public NetworkPeer ClientPeer = null;

	// Use this for initialization
	void Start () 
	{
		NetworkConnector.Connector.LocalClientConnected += Connector_LocalClientConnected;
	}

	void Connector_LocalClientConnected(object sender, System.EventArgs e)
	{
		throw new System.NotImplementedException();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
