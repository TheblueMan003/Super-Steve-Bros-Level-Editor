using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourcesManager : MonoBehaviour
{
    public BlockManager manager;
    public List<Sprite> sprites;

    public void Reload(string path)
    {
        Dictionary<string, Sprite> dic = new Dictionary<string, Sprite>();

        foreach (string file in Directory.GetFiles(path+"\\"+"block"))
        {
            dic.Add(GetName(file), GetSprite(file));
        }
        foreach (string file in Directory.GetFiles(path + "\\" + "item"))
        {
            dic.Add(GetName(file), GetSprite(file));
        }

        foreach (placable s in manager.tiles)
        {
            if (dic.ContainsKey(s.icon_name))
            {
                s.icon = dic[s.icon_name];
            }

            if (s.tile != null && dic.ContainsKey(s.tile.name))
            {
                s.tile.sprite = dic[s.tile.name];
            }

            if (s.ruleTile != null && !Application.isEditor)
            {
                foreach(var p in s.ruleTile.m_TilingRules)
                {
                    if (p.m_Sprites.Length > 0 && p.m_Sprites[0] != null && dic.ContainsKey(p.m_Sprites[0].name))
                    {
                        p.m_Sprites[0] = dic[p.m_Sprites[0].name];
                    }
                }
            }
        }
    }
    public string GetName(string file)
    {
        return file.Substring(file.LastIndexOf('\\') + 1, file.Length - file.LastIndexOf('\\') - 1).Replace(".png", "");
    }

    public Sprite GetSprite(string file)
    {
        var bytes = File.ReadAllBytes(file);
        Texture2D texture = new Texture2D(16, 16);
        texture.filterMode = FilterMode.Point;

        texture.LoadImage(bytes);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, Mathf.Min(texture.width, texture.height)), new Vector2(0.5f, .5f), 16);
    }
}
