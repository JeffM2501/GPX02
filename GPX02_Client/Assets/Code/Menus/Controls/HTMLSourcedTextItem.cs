using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class HTMLSourcedTextItem : MonoBehaviour
{
    public string URL = string.Empty;

    protected Text TextArea = null;
    protected WWW WebForm = null;
    protected bool Processing = false;

	void Start ()
    {
        TextArea = gameObject.GetComponent<Text>();

        if (TextArea != null)
            UpdateFromURL();
	}

    public void UpdateFromURL()
    {
        if (URL != string.Empty)
        {
            Processing = true;
            WebForm = new WWW(URL);
        }
    }

	void Update ()
    {
	    if (Processing && WebForm.isDone)
        {
            if (TextArea != null)
                TextArea.text = WebForm.text;
            Processing = false;
            WebForm = null;
        }
	}
}