using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public List<placable> tiles;
    public Dictionary<placable, int> tiles_Set;

    public void Start()
    {
        tiles_Set = new Dictionary<placable, int>();
        int i =  0;
        foreach (placable item in tiles)
        {
            tiles_Set.Add(item, i);
            i++;
        }
    }
}
