using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveStat
{
    public LevelSaved[] levels;

    public SaveStat(LevelSaved[] levels)
    {
        this.levels = levels;
    }

    public static string FromList(BlockManager blockManager, List<Level> levels)
    {
        var nLevel = new LevelSaved[levels.Count];

        for (int i = 0; i < levels.Count; i++)
        {
            nLevel[i] = levels[i].GetSave(blockManager);
        }

        return JsonUtility.ToJson(new SaveStat(nLevel));
    }

    public static List<Level> FromJson(BlockManager blockManager, string text)
    {
        var parsed = JsonUtility.FromJson<SaveStat>(text);
        List<Level> levels = new List<Level>();

        foreach (LevelSaved level in parsed.levels)
        {
            levels.Add(level.GetLevel(blockManager));
        }

        return levels;
    }
}

[System.Serializable]
public class LevelSaved
{
    public string[] areas;

    public LevelSaved(string[] areas)
    {
        this.areas = areas;
    }

    public Level GetLevel(BlockManager blockManager)
    {
        List<Level.Area> areas2 = new List<Level.Area>();
        foreach (string area in areas)
        {
            areas2.Add(Level.Area.FromString(blockManager, area));
        }

        Level lvl = new Level();
        lvl.areas = areas2;

        return lvl;
    }
}