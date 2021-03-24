using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public List<Area> areas = new List<Area>();

    public static int mapH = 32;
    public static int mapW = 256;
    public static int areaNumber = 4;

    public Level()
    {
        for (int i = 0; i < areaNumber; i++)
        {
            areas.Add(new Area());
        }
    }

    public LevelSaved GetSave(BlockManager blockManager)
    {
        string[] areas2 = new string[areas.Count];

        for (int i = 0; i < areas.Count; i++)
        {
            areas2[i] = areas[i].GetSave(blockManager);
        }

        return new LevelSaved(areas2);
    }

    public class Area
    {
        public placable[,,] cell;
        public bool night;
        public int music;
        private static string codex = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public Area()
        {
            cell = new placable[mapW, mapH, 2];
            night = false;
        }

        public string GetSave(BlockManager blockManager)
        {
            string save = "";
            save += night ? "n" : "d";
            save += ";" + music.ToString();

            save += ',';
            for (int y = 0; y < mapH; y++)
            {
                for (int x = 0; x < mapW; x++)
                {
                    for (int l = 0; l < 2; l++)
                    {
                        if (cell[x, y, l] != null)
                        {
                            save += compressBlock(blockManager.tiles_Set[cell[x, y, l]]) + ";";
                        }
                        else
                        {
                            save += ";";
                        }
                    }
                }
            }

            return save;
        }

        private static string compressBlock(int value)
        {
            if (value >= 0 && value < codex.Length)
            {
                return codex[value] + "";
            }
            else if (value >= codex.Length)
            {
                return compressBlock(value / codex.Length) + compressBlock(value % codex.Length);
            }
            else
            {
                return "";
            }
        }

        private static int uncompressBlock(string value)
        {
            if (value == "" || value == null)
            {
                return -1;
            }
            int output = 0;

            for (int i = 0; i < value.Length; i++)
            {
                output += (int)(codex.IndexOf(value[i]) * Mathf.Pow(codex.Length, value.Length - i - 1));
            }

            return output;
        }

        public static Area FromString(BlockManager blockManager, string arg)
        {
            Area area = new Area();
            var lines = arg.Split(',');
            string[] c = lines[0].Split(';');
            area.night = (c[0] == "n");

            if (c.Length > 1)
            {
                area.music = (int.Parse(c[1]));
            }
            else
            {
                area.music = 0;
            }

            area.cell = (new placable[mapW, mapH, 2]);

            int x = 0;
            int y = 0;
            int l = 0;
            bool stop = false;

            foreach (string tile in lines[1].Split(';'))
            {
                if (!stop)
                {
                    int block = uncompressBlock(tile);

                    area.cell[x, y, l] = (block > -1) ? blockManager.tiles[block] : null;

                    l++;
                    if (l >= 2)
                    {
                        l = 0;
                        x++;
                        if (x >= mapW)
                        {
                            x = 0;
                            y++;
                            if (y >= mapH)
                            {
                                stop = true;
                            }
                        }
                    }
                }
            }

            return area;
        }
    }
}
