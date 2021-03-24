using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class placable : MonoBehaviour
{
    public Tile tile;
    public Sprite icon;
    public string icon_name;
    public RuleTile ruleTile;
    public int layer;

    public new string name;
    public string mcName;

    public bool seizable;
    public int maxWidth;
    public int maxHeight;

    public int pos;
    public bool structured;
    public structureCell[] cells;
    public bool shifted;

    public abstract string getCmd(int x, int y, int layer, placable[,,] cell);
}

[System.Serializable]
public class structureCell
{
    public placable item;
    public Vector3Int pos;
}