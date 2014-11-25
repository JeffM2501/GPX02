using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ToggleEventHandler : MonoBehaviour
{
    public event EventHandler Toggled;

	void Start ()
    {
	    
	}

	void Update ()
    {
	
	}

    public void ToggledCallback()
    {
        if (Toggled != null)
            Toggled(this, EventArgs.Empty);
    }
}