//GPX02
// Copyright 2014 Jeffery Myers

using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GameLevel : MonoBehaviour
{
    public string LevelName = string.Empty;

    public bool StartedServer = false;

	void Start ()
    {
		Debug.Log("Game Level Started");

        if (Application.isLoadingLevel)
            Debug.Log("Is loading");

		if (Application.isEditor && !Application.isLoadingLevel)
			OnLevelWasLoaded(Application.loadedLevel);
	}

    void OnLevelWasLoaded (int level)
    {
        if (StartedServer)
            return;

      //  if (level >= 0 && level < UnityEditor.EditorBuildSettings.scenes.Length)
       //     LevelName = Path.GetFileName(UnityEditor.EditorBuildSettings.scenes[level].path);
		Debug.Log("OnLevel Was Loaded " + LevelName);
        if (NetworkConnector.Connector != null)
			NetworkConnector.Connector.Startup(this);

		StartedServer = NetworkConnector.Connector != null;
    }

	void Update ()
    {
	
	}
}