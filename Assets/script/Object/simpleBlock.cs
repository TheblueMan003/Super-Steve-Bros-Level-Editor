using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleBlock : placable
{
    public string blockID;
    public conditionPlace[] conds;
    
    public override string getCmd(int x, int y, int layer, placable[,,] cell)
    {
        if (blockID == "")
        {
            return "";
        }
        else
        {
            string add = "";

            if (layer == 0)
            {
                add = '\n'+"setblock ~" + (x + 1).ToString() + " ~" + (y + 1).ToString() + " ~10 minecraft:barrier";
            }
            if (seizable)
            {
                bool[,] stat = getSide(x, y, layer, cell);
                string output = "setblock ~" + (x + 1).ToString() + " ~" + (y + 1).ToString() + " ~" + (-layer).ToString() + " minecraft:" + conds[conds.Length - 1].id + add;
                
                foreach (conditionPlace cond in conds)
                {
                    if (cond.isValid(stat))
                    {
                        if (cond.id.StartsWith("{"))
                            output = "summon armor_stand" + " ~" + (x + 1).ToString() + " ~" + (y + 1).ToString() + " ~ " + cond.id;
                        else
                            output = "setblock ~" + (x + 1).ToString() + " ~" + (y + 1).ToString() + " ~" + (-layer).ToString() + " minecraft:" + cond.id + add;
                    }
                }
                return output;
            }
            else
            {
                return "setblock ~" + (x + 1).ToString() + " ~" + (y + 1).ToString() + " ~" + (-layer).ToString() + " minecraft:" + blockID + add;
            }
        }
    }

    public bool[,] getSide(int x, int y, int layer, placable[,,] cell)
    {
        var output = new bool[3,3];
        if (x-1 >= 0)
        {
            if (y-1 >= 0) output[0, 2] = cell[x - 1, y - 1, layer] == this;
            output[0, 1] = cell[x - 1, y, layer] == this;
            if (y + 1 < RayController.mapH) output[0, 0] = cell[x - 1, y + 1, layer] == this;
        }

        if (y - 1 >= 0) output[1, 2] = cell[x, y - 1, layer] == this;
        output[1, 1] = cell[x, y, layer] == this;
        if (y + 1 < RayController.mapH) output[1, 0] = cell[x, y + 1, layer] == this;

        if (x + 1 < RayController.mapW)
        {
            if (y - 1 >= 0) output[2, 2] = cell[x + 1, y - 1, layer] == this;
            output[2, 1] = cell[x + 1, y, layer] == this;
            if (y + 1 < RayController.mapH) output[2, 0] = cell[x + 1, y + 1, layer] == this;
        }

        return output;
    }
}

[System.Serializable]
public class conditionPlace
{
    public Vector2[] conds;
    public string id;

    public bool isValid(bool[,] cell)
    {
        foreach (Vector2 cond in conds)
        {
            if (cond.x == -1)
            {
                if (cond.y == -1 && !cell[0, 2])
                {
                    return false;
                }
                if (cond.y == 0 && !cell[0, 1])
                {
                    return false;
                }
                if (cond.y == 1 && !cell[0, 0])
                {
                    return false;
                }
            }
            if (cond.x == 0)
            {
                if (cond.y == -1 && !cell[1, 2])
                {
                    return false;
                }
                if (cond.y == 0 && !cell[1, 1])
                {
                    return false;
                }
                if (cond.y == 1 && !cell[1, 0])
                {
                    return false;
                }
            }
            if (cond.x == 1)
            {
                if (cond.y == -1 && !cell[2, 2])
                {
                    return false;
                }
                if (cond.y == 0 && !cell[2, 1])
                {
                    return false;
                }
                if (cond.y == 1 && !cell[2, 0])
                {
                    return false;
                }
            }
        }
        return true;
    }
}