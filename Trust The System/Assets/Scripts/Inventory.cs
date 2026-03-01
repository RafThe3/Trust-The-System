using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public List<GameObject> items = new();
    public int space = 1;

    private void Awake()
    {
        if (Instance)
        {
            Debug.Log("More than one instance of Inventory found!");
            return;
        }

        Instance = this;
    }

    public virtual bool Add(GameObject item)
    {
        if (items.Count >= space)
        {
            Debug.LogError("Not enough room!");
            return false;
        }

        items.Add(item);
        return true;
    }

    public void Remove(GameObject item)
    {
        items.Remove(item);
    }
}
