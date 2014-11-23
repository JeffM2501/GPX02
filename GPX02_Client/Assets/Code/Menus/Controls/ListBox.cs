using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ListBox : MonoBehaviour
{
    public Toggle ItemPrefab = null;
    public ScrollRect ScrollArea = null;

    protected class ListItem
    {
        public GameObject RawObject = null;
        public Toggle Item = null;
        public object Tag = null;
    }

    protected List<ListItem> Items = new List<ListItem>();
    protected ToggleGroup ItemGroup = null;

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
        }
        item.Tag = tag;

        Items.Add(item);

        ScrollArea.verticalNormalizedPosition = 0;
    }

    public void ClearItems()
    {
        foreach(ListItem item in Items)
            GameObject.Destroy(item.RawObject);

        Items.Clear();
    }


}