using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static string currentSave;
    public static bool isLoadingSave;
    public static string GameMode = "smb" ;
    
    public static void LoadSave(string save)
    {
        currentSave = save;
        string[] lines = File.ReadAllLines(Application.dataPath + "/../saves/" + save + ".smk");

        try
        {
            GameMode = lines[0].Split('-')[1];
            isLoadingSave = true;
        }
        catch
        {
            GameMode = "smb";
            isLoadingSave = false;
        }
    }


}
