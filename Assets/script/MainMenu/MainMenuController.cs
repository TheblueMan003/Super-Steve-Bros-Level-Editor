using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static float scrollIndex;
    public static int saveNb;

    public Scrollbar scrollbar;
    public int csaveNb;
    public GameObject saveTemplate;
    public GameObject parent;
    public List<GameObject> saves;
    public Text version;
    public GameObject WorldTypeSelection;

    void Start()
    {
        WorldTypeSelection.SetActive(false);
        saveNb = csaveNb;
        Reload();
        version.text = "version: "+Application.version;

        while (PlayerPrefs.GetInt("USER_ONLINE_ID", 0) == 0)
        {
            PlayerPrefs.SetInt("USER_ONLINE_ID", Random.Range(int.MinValue, int.MaxValue));
        }
    }

    void Update()
    {
        scrollIndex = scrollbar.value;
    }

    public void Reload()
    {
        if (saves != null) {
            foreach (GameObject go in saves)
            {
                Destroy(go);
            }
        }
        saves = new List<GameObject>();

        if (Directory.Exists(Application.dataPath + "/../saves/"))
        {
            foreach (string file in Directory.GetFiles(Application.dataPath + "/../saves/"))
            {
                var p = Instantiate(saveTemplate, parent.transform);
                p.GetComponent<LoadSaveButton>().init(saves.Count, file, File.GetCreationTime(file).ToString());
                saves.Add(p);
            }
        }

        saveNb = saves.Count+1;

        scrollbar.size = 1f/Mathf.Max(1, (saveNb-4));
    }

    public void NewWorld()
    {
        //SceneManager.LoadScene("SMB1");
        WorldTypeSelection.SetActive(true);
    }

    public void NewWorldType(string scene)
    {
        SaveManager.GameMode = scene;
        SceneManager.LoadScene("SMB1");
    }
}

