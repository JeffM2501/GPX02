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

	void Start ()
    {
		Debug.Log("Game Level Started");
		if (Debug.isDebugBuild)
			OnLevelWasLoaded(0);
	}

    void OnLevelWasLoaded (int level)
    {
        LevelName = Path.GetFileName(UnityEditor.EditorBuildSettings.scenes[level].path);
		Debug.Log("OnLevel Was Loaded " + LevelName);
        if (NetworkHost.Host != null)
            NetworkHost.Host.Startup(this);
    }

	void Update ()
    {
	
	}
}