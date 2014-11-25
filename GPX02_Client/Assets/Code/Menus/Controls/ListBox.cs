//GPX02
// Copyright 2014 Jeffery Myers

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ListBox : MonoBehaviour
{
    public Toggle ItemPrefab = null;
    public ScrollRect ScrollArea = null;

    public class ListItem
    {
        public GameObject RawObject = null;
        public Toggle Item = null;
        public object Tag = null;
    }

    protected List<ListItem> Items = new List<ListItem>();
    protected ToggleGroup ItemGroup = null;

    protected ListItem SelectedItem = null;
    
    public ListItem GetSelectedItem()
    {
        return SelectedItem;
    }

    public object GetSelectedItemTag()
    {
        return SelectedItem == null ? null : SelectedItem.Tag;
    }

    public event EventHandler SelectionChanged = null;

	void Start ()
    {
        if (ScrollArea != null)
        {
            ItemGroup = ScrollArea.gameObject.GetComponent<ToggleGroup>();
            if (ItemGroup == null && ScrollArea.content != null)
                ItemGroup = ScrollArea.content.gameObject.GetComponent<ToggleGroup>();

            ScrollArea.verticalNormalizedPosition = 1;
        }
    }

	void Update ()
    {
    }

    public void AddItem(string labelName, object tag)
    {
        if (ScrollArea == null || ScrollArea.content == null)
            return;

        ListItem item = new ListItem();

        item.RawObject = GameObject.Instantiate(ItemPrefab.gameObject) as GameObject;
        item.RawObject.transform.SetParent(ScrollArea.content, false);

        item.Item = item.RawObject.GetComponent<Toggle>();
        if (item.Item != null)
        {
            item.Item.isOn = false;
            item.Item.group = ItemGroup;

            Text t = item.Item.GetComponentInChildren<Text>();
            if (t != null)
                t.text = labelName;

            ToggleEventHandler evt = item.RawObject.GetComponent<ToggleEventHandler>();
            if (evt != null)
                evt.Toggled += evt_Toggled;
        }
        item.Tag = tag;

        Items.Add(item);

        ScrollArea.verticalNormalizedPosition = 0;
    }

    void evt_Toggled(object sender, EventArgs e)
    {
        ToggleEventHandler handler = sender as ToggleEventHandler;
        if (handler != null)
            SelectedItem = Items.Find(delegate(ListItem i) { return i.RawObject == handler.gameObject; });

        if (SelectionChanged != null)
            SelectionChanged(this,EventArgs.Empty);
    }

    public void ClearItems()
    {
        foreach(ListItem item in Items)
            GameObject.Destroy(item.RawObject);

        Items.Clear();
    }


}