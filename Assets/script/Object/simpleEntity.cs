using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleEntity : placable
{
    public string entity;
    public string data;

    public override string getCmd(int x, int y, int layer, placable[,,] cell)
    {
        float s = shifted ? 0.5f : 0;
        return "summon " + entity + " ~" + (x+1+s).ToString() + " ~" + (y+1).ToString() + " ~ " + data;
    }
}
