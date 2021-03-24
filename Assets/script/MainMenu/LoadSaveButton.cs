using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class LoadSaveButton : MonoBehaviour
{
    public int index;
    public float size;
    public float x_shift;
    public float y_shift;
    public float y_top;

    private RectTransform rect;

    public Text saveName;
    public Text saveDate;
    public Image imageIcon;

    public List<string> GameMode;
    public List<string> Scenes;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.localPosition = new Vector3(x_shift, y_shift+(MainMenuController.scrollIndex * (MainMenuController.saveNb-4) - index) * size, 0);
    }

    public void init(int index, string name, string date)
    {
        this.index = index;
        saveDate.text = date;
        saveName.text = name.Substring(name.LastIndexOf("/")+1, name.LastIndexOf(".") - name.LastIndexOf("/")-1);

        string[] text = File.ReadAllLines(Application.dataPath + "/../saves/" + saveName.text + ".smk");
        var imgL = text[text.Length - 1];

        if (imgL.StartsWith("img:"))
        {
            byte[] img = Convert.FromBase64String(imgL.Replace("img:", "").Replace('\n'+"",""));
            var t = new Texture2D(1, 1);
            t.LoadImage(img, false);

            imageIcon.sprite = Sprite.Create(t, new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f));
        }
    }

    public void Press()
    {
        SaveManager.LoadSave(saveName.text);

        SceneManager.LoadScene("SMB1");
    }
}
