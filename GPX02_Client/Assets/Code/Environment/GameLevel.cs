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
        //
	}

    void OnLevelWasLoaded (int level)
    {
        LevelName = Path.GetFileName(UnityEditor.EditorBuildSettings.scenes[level].path);
        if (NetworkHost.Host != null)
            NetworkHost.Host.Startup(this);
    }

	void Update ()
    {
	
	}
}