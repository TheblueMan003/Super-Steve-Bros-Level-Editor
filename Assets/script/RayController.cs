using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.IO.Compression;
using UnityEngine.UI;
using System;

public class RayController : MonoBehaviour
{
    private string codex = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public GameObject Camera_OBJ;
    public int fill_distance;
    public Tilemap tilemapFore;
    public Tilemap tilemapBack;
    public Tilemap tilemapAnim;

    public placable currentBlock;
    public List<placable> lastBlocks;

    public List<MenuObject> blockLst;
    public int currentMenu;
    public bool isMenuOpen;
    public bool isSaveOpen;
    public bool isDialogOpen;

    public static int mapH = 32;
    public static int mapW = 256;
    public List<Level> level = new List<Level>();
    
    public float speed;
    private bool multiplacing;
    private Vector3Int mp_start;
    public GameObject MenuPannel;
    public GameObject SavePannel;
    public GameObject[] SavePannels;
    public GameObject DialogPannel;
    public GameObject gridObj;

    public Text dialogText;
    public Text LevelIndicator;

    public InputField directoryLabel;
    public string directoryText;

    public InputField fileLabel;
    public string fileName;

    public Toggle zipToggle;
    public Toggle nightToggle;
    public Toggle autofillToggle;
    public Dropdown musicDropdown;

    //public List<bool> night = new List<bool>();
    //public List<int> musics = new List<int>();
    public bool zip;
    public bool grid;
    public bool autofill;

    public string[] character;
    public int selectedChar;
    public string[] armor;
    public Image[] charButton;

    public int savePannelIndex;
    public int currentLevel;
    public int currentArea;
    public Tile blackTile;

    public BlockManager blockManager;
    public string gameMode = "smb";

    public ResourcesManager resourcesManager;
    public Camera screenshotCam;

    void Start()
    {
        level.Add(new Level());
        //night.Add(false);
        //musics.Add(0);

        blockManager.Start();

        if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +"/.minecraft/saves"))
        {
            directoryLabel.text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +"/.minecraft/saves";
            directoryText = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.minecraft/saves";
            zipToggle.isOn = false;
            zip = false;
        }
        else
        {
            directoryLabel.text = Application.dataPath;
            directoryText = Application.dataPath;
            zipToggle.isOn = true;
            zip = true;
        }
        selectChar(0);

        if (SaveManager.isLoadingSave)
        {
            loadFile(SaveManager.currentSave);
            SaveManager.isLoadingSave = false;
        }
        Debug.Log(resourcesManager);
        Debug.Log(SaveManager.GameMode);
        if (!Application.isEditor)
            resourcesManager.Reload(Application.dataPath + "/../resources/" + SaveManager.GameMode + "/assets/minecraft/textures");
        reloadLevel();
    }

    public void Update()
    {
        MenuPannel.SetActive(isMenuOpen);
        SavePannel.SetActive(isSaveOpen);
        DialogPannel.SetActive(isDialogOpen);
        gridObj.SetActive(grid);

        bool filling = Input.GetKey(KeyCode.F) || autofill;

        for (int i = 0; i < SavePannels.Length; i++)
        {
            SavePannels[i].SetActive(i == savePannelIndex && isSaveOpen);
        }

        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.LeftControl))
        {
            SaveMCFunction();
        }

        if (!isMenuOpen && !isSaveOpen && !isDialogOpen)
        {
            transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * speed;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit = new RaycastHit();
            if (Input.GetMouseButtonDown(0))
            {
                Vector3Int tmp_pos;
                if (Physics.Raycast(ray, out hit) && (hit.point - Camera_OBJ.transform.position).magnitude < fill_distance)
                {
                    var tmp = hit.collider.gameObject;
                    if (tmp.tag == "placable")
                    {
                        tmp_pos = tilemapFore.WorldToCell(hit.point);
                        if (currentBlock.seizable || filling)
                        {
                            multiplacing = true;
                            mp_start = tmp_pos;
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector3Int tmp_pos;
                if (Physics.Raycast(ray, out hit) && (hit.point - Camera_OBJ.transform.position).magnitude < fill_distance)
                {
                    var tmp = hit.collider.gameObject;
                    if (tmp.tag == "placable")
                    {
                        tmp_pos = tilemapFore.WorldToCell(hit.point);
                        if ((currentBlock.seizable || filling) && multiplacing)
                        {
                            multiplacing = false;

                            tilemapAnim.ClearAllTiles();
                            for (int x = mp_start.x; x != tmp_pos.x;)
                            {
                                for (int y = mp_start.y; y != tmp_pos.y;)
                                {
                                    if (setCell(x, y, currentBlock))
                                    {
                                        if (currentBlock.layer == 0)
                                        {
                                            if (currentBlock.seizable)
                                                tilemapFore.SetTile(new Vector3Int(x, y, 0), currentBlock.ruleTile);
                                            else
                                                tilemapFore.SetTile(new Vector3Int(x, y, 0), currentBlock.tile);
                                        }
                                        else
                                        {
                                            if (currentBlock.seizable)
                                                tilemapBack.SetTile(new Vector3Int(x, y, 0), currentBlock.ruleTile);
                                            else
                                                tilemapBack.SetTile(new Vector3Int(x, y, 0), currentBlock.tile);
                                        }
                                    }
                                    y = y < tmp_pos.y ? y + 1 : y - 1;
                                }
                                x = x < tmp_pos.x ? x + 1 : x - 1;
                            }
                        }
                    }
                }
            }

            if (Input.GetMouseButton(0))
            {
                Vector3Int tmp_pos;
                if (Physics.Raycast(ray, out hit) && (hit.point - Camera_OBJ.transform.position).magnitude < fill_distance)
                {
                    var tmp = hit.collider.gameObject;
                    if (tmp.tag == "placable")
                    {
                        tmp_pos = tilemapFore.WorldToCell(hit.point);
                        if (!currentBlock.seizable && !filling)
                        {
                            if (setCell(tmp_pos.x, tmp_pos.y, currentBlock))
                            {
                                if (!currentBlock.structured)
                                { 
                                    if (currentBlock.layer == 0)
                                    {
                                        tilemapFore.SetTile(tilemapFore.WorldToCell(hit.point), currentBlock.tile);
                                    }
                                    else
                                    {
                                        tilemapBack.SetTile(tilemapFore.WorldToCell(hit.point), currentBlock.tile);
                                    }
                                }
                                else
                                {
                                    foreach (structureCell cell in currentBlock.cells)
                                    {
                                        if (setCell(tmp_pos.x + cell.pos.x, tmp_pos.y + cell.pos.y, cell.item))
                                        {
                                            if (currentBlock.layer == 0)
                                            {
                                                tilemapFore.SetTile(tilemapFore.WorldToCell(hit.point) + cell.pos, cell.item.tile);
                                            }
                                            else
                                            {
                                                tilemapBack.SetTile(tilemapFore.WorldToCell(hit.point) + new Vector3Int(cell.pos.x, cell.pos.y, 0), cell.item.tile);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (multiplacing)
                        {
                            tilemapAnim.ClearAllTiles();
                            for (int x = mp_start.x; x != tmp_pos.x;)
                            {
                                for (int y = mp_start.y; y != tmp_pos.y;)
                                {
                                    if (currentBlock.seizable)
                                        tilemapAnim.SetTile(new Vector3Int(x, y, 0), currentBlock.ruleTile);
                                    else
                                        tilemapAnim.SetTile(new Vector3Int(x, y, 0), currentBlock.tile);
                                    y = y < tmp_pos.y ? y + 1 : y - 1;
                                }
                                x = x < tmp_pos.x ? x + 1 : x - 1;
                            }
                        }
                    }
                }
            }

            if (Input.GetMouseButton(1))
            {
                Vector3Int tmp_pos;
                if (Physics.Raycast(ray, out hit) && (hit.point - Camera_OBJ.transform.position).magnitude < fill_distance)
                {
                    var tmp = hit.collider.gameObject;
                    if (tmp.tag == "placable")
                    {
                        tmp_pos = tilemapFore.WorldToCell(hit.point);
                        if (emptyCell(tmp_pos.x, tmp_pos.y))
                        {
                            tilemapFore.SetTile(tilemapFore.WorldToCell(hit.point), null);
                            tilemapBack.SetTile(tilemapFore.WorldToCell(hit.point), null);
                        }
                    }
                }
            }
        }
    }

    public bool setCell(int x, int y, placable value)
    {
        if (x >= 0 && x < mapW && y >= 0 && y < mapH)
        {
            level[currentLevel].areas[currentArea].cell[x,y, value.layer] = value;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool emptyCell(int x, int y)
    {
        if (x >= 0 && x < mapW && y >= 0 && y < mapH)
        {
            level[currentLevel].areas[currentArea].cell[x, y, 0] = null;
            level[currentLevel].areas[currentArea].cell[x, y, 1] = null;
            return true;
        }
        else
        {
            return false;
        }
    }
    public placable getCell(int x, int y, int l)
    {
        return level[currentLevel].areas[currentArea].cell[x, y, l];
    }
    
    public void SaveMCFunction()
    {
        
        List<string> main = new List<string>();
        List<string> tick = new List<string>();

        for (int k = 0; k < level.Count; k++)
        {
            int sub = 0;
            foreach (Level.Area area in level[k].areas)
            {
                var data = new List<string>();
                if (sub == 0)
                {
                    data.Add("function main:super_mario_bros/music/set_music_" + area.music.ToString());
                    data.Add("scoreboard players set @a Music_Tick -1");
                    data.Add("stopsound @a");
                }

                tick.Add("execute as @a[scores={Level=" + k.ToString() +"}, z=" + (10 - (sub * 20)).ToString() + ",dz=3] run function main:super_mario_bros/music/set_music_" + area.music.ToString());

                data.Add("fill ~1 ~1 ~ ~256 ~33 ~ minecraft:air");
                data.Add("fill ~1 ~1 ~10 ~256 ~33 ~10 minecraft:air");

                if (!area.night)
                {
                    data.Add("fill ~1 ~1 ~-1 ~256 ~33 ~-1 minecraft:sea_lantern");
                }
                else
                {
                    data.Add("fill ~1 ~1 ~-1 ~256 ~33 ~-1 minecraft:glowstone");
                }

                for (int y = 0; y < mapH; y++)
                {
                    for (int x = 0; x < mapW; x++)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            if (area.cell[x, y, l] != null)
                            {
                                data.Add(area.cell[x, y, l].getCmd(x, y, l, area.cell));
                            }
                        }
                    }
                }

                if (sub == 0)
                {
                    for (int i = 1; i < level[k].areas.Count; i++)
                    {
                        data.Add("execute positioned ~ ~ ~-" + (i * 20).ToString() + " run function main:levels/level_" + k.ToString() + "_" + i.ToString());
                    }

                    File.WriteAllLines(Application.dataPath + "/../data/datapacks/TheblueMan003/data/main/functions/levels/level_" + k.ToString() + ".mcfunction", data.ToArray());
                    main.Add("execute if entity @a[scores={Level=" + k.ToString() + "}] run function main:levels/level_" + k.ToString());
                }
                else
                {
                    File.WriteAllLines(Application.dataPath + "/../data/datapacks/TheblueMan003/data/main/functions/levels/level_" + k.ToString()+"_"+ sub.ToString() + ".mcfunction", data.ToArray());
                }
                sub++;
            }
        }

        main.Add("execute if entity @a[scores={Level=" + level.Count.ToString() + "}] run execute in minecraft:overworld run tp @a -34.64 23.00 59.84 -179.69 5.16");
        main.Add("execute if entity @a[scores={Level=" + level.Count.ToString() + "}] run tag @a remove PlayingIn2D");
        main.Add("execute if entity @a[scores={Level=" + level.Count.ToString() + "}] run function smb_extra:main/hubreload");

        File.WriteAllLines(Application.dataPath + "/../data/datapacks/TheblueMan003/data/main/functions/levels/main.mcfunction", main.ToArray());
        File.WriteAllLines(Application.dataPath + "/../data/datapacks/TheblueMan003/data/main/functions/levels/tick.mcfunction", tick.ToArray());

        File.WriteAllBytes(Application.dataPath + "/../data/icon.png", TakeScreenshotWorldIcon());
        /*
        for (int i = 0; i < armor.Length; i++)
        {
            if (File.Exists(Application.dataPath + "/../data/resources/assets/minecraft/textures/models/armor/" + armor[i] + ".png"))
            {
                File.Delete(Application.dataPath + "/../data/resources/assets/minecraft/textures/models/armor/" + armor[i] + ".png");
            }
            File.Copy(Application.dataPath + "/../data/resources/assets/minecraft/textures/models/armor/" + character[selectedChar] + "_" + (i+1).ToString() + ".png",
                Application.dataPath + "/../data/resources/assets/minecraft/textures/models/armor/" + armor[i]+".png");
        }
        */
        if (File.Exists(Application.dataPath + "/../data/resources.zip"))
        {
            File.Delete(Application.dataPath + "/../data/resources.zip");
        }

        ZipFile.CreateFromDirectory(Application.dataPath + "/../resources/"+SaveManager.GameMode, Application.dataPath + "/../data/resources.zip");
        
        if (zip)
        {
            ZipFile.CreateFromDirectory(Application.dataPath + "/../data", directoryText + "/" + fileName + ".zip");
        }
        else
        {
            foreach (string dirPath in Directory.GetDirectories(Application.dataPath + "/../data", "*", SearchOption.AllDirectories)) {
                Directory.CreateDirectory(dirPath.Replace(Application.dataPath + "/../data", directoryText + "/" + fileName));
            }

            foreach (string newPath in Directory.GetFiles(Application.dataPath + "/../data", "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(Application.dataPath + "/../data", directoryText + "/" + fileName), true);
            }
        }

        ShowDialog("Your map have been exported!");
    }

    public void NightToggle()
    {
        level[currentLevel].areas[currentArea].night = nightToggle.isOn;
        GetComponent<Camera>().backgroundColor = level[currentLevel].areas[currentArea].night ? new Color(0.1f, 0.1f, 0.1f) : new Color(150 / 255f, 244 / 255f, 1);
    }

    public void ZipToggle()
    {
        zip = !zip;
    }

    public void UpdateDirectory()
    {
        directoryText = directoryLabel.text;
    }
    public void UpdateFilename()
    {
        fileName = fileLabel.text;
    }

    public void ShowDialog(string text)
    {
        isDialogOpen = true;
        isSaveOpen = false;
        isMenuOpen = false;

        DialogPannel.SetActive(true);
        dialogText.text = text;
    }

    public void CloseDialog()
    {
        isDialogOpen = false;
    }

    public void resetLevel()
    {
        level[currentLevel].areas[currentArea].cell = new placable[mapW, mapH, 2];
        tilemapFore.ClearAllTiles();
        tilemapBack.ClearAllTiles();
    }

    public void gridToggleA()
    {
        grid = !grid;
    }

    public void autoFillA()
    {
        autofill = !autofill;
    }

    public void changeMusic()
    {
        level[currentLevel].areas[currentArea].music = musicDropdown.value;
    }

    public void selectChar(int index)
    {
        selectedChar = index;
        int i = 0;
        foreach(Image image in charButton)
        {
            if (i == index)
            {
                image.color = Color.cyan;
            }
            else
            {
                image.color = Color.white;
            }
            i++;
        }
    }

    public void selectMenu(int index)
    {
        savePannelIndex = index;
    }

    public void nextLevel()
    {
        if (currentLevel < level.Count-1)
            currentLevel += 1;
        reloadLevel();
    }

    public void prevLevel()
    {
        if (currentLevel > 0)
            currentLevel -= 1;
        reloadLevel();
    }

    public void newLevel()
    {
        level.Add(new Level());

        currentLevel = level.Count - 1;
        reloadLevel();
    }

    public void deleteLevel()
    {
        level.RemoveAt(currentLevel);

        if (currentLevel > 0)
            currentLevel -= 1;

        if (level.Count == 0)
            newLevel();
        else
            reloadLevel();
    }

    public void reloadLevel()
    {
        nightToggle.isOn = level[currentLevel].areas[currentArea].night;
        musicDropdown.value = level[currentLevel].areas[currentArea].music;
        GetComponent<Camera>().backgroundColor = level[currentLevel].areas[currentArea].night ? new Color(0f, 0f, 0f) : new Color(92 / 255f, 148 / 255f, 252/255f);
        
        tilemapBack.ClearAllTiles();
        tilemapFore.ClearAllTiles();

        LevelIndicator.text = (currentLevel + 1).ToString() + "/" + level.Count.ToString();

        for (int i = -32; i < mapW+32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                tilemapFore.SetTile(new Vector3Int(i, -j - 1, 0), blackTile);
                tilemapFore.SetTile(new Vector3Int(i, 32+j, 0), blackTile);
            }
        }
        for (int i = 0; i < mapH; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                tilemapFore.SetTile(new Vector3Int(-j-1, i, 0), blackTile);
                tilemapFore.SetTile(new Vector3Int(mapW+j, i, 0), blackTile);
            }
        }

        for (int y = 0; y < mapH; y++)
        {
            for (int x = 0; x < mapW; x++)
            {
                for (int l = 0; l < 2; l++)
                {
                    if (l == 0)
                    {
                        if (level[currentLevel].areas[currentArea].cell[x, y, l] != null)
                        {
                            if (level[currentLevel].areas[currentArea].cell[x, y, l].seizable)
                            {
                                tilemapFore.SetTile(new Vector3Int(x, y, 0), level[currentLevel].areas[currentArea].cell[x, y, l].ruleTile);
                            }
                            else
                            {
                                tilemapFore.SetTile(new Vector3Int(x, y, 0), level[currentLevel].areas[currentArea].cell[x, y, l].tile);
                            }
                        }
                    }
                    else
                    {
                        if (level[currentLevel].areas[currentArea].cell[x, y, l] != null)
                        {
                            if (level[currentLevel].areas[currentArea].cell[x, y, l].seizable)
                            {
                                tilemapBack.SetTile(new Vector3Int(x, y, 0), level[currentLevel].areas[currentArea].cell[x, y, l].ruleTile);
                            }
                            else
                            {
                                tilemapBack.SetTile(new Vector3Int(x, y, 0), level[currentLevel].areas[currentArea].cell[x, y, l].tile);
                            }
                        }
                    }
                }
            }
        }
    }

    public void saveFile()
    {
        string save = "";
        int i = 0;
        save += "1-"+SaveManager.GameMode+'\n'; //version - game mode
        Debug.Log(blockManager);
        Debug.Log(blockManager.tiles);
        save += selectedChar.ToString() + ";";
        save += '\n';

        save += SaveStat.FromList(blockManager, level);
        /*
        foreach (placable[,,] level in cell)
        {
            save += night[i] ? "n" : "d";
            save += ";" + musics[i].ToString();

            save += '\n';
            for (int y = 0; y < mapH; y++)
            {
                for (int x = 0; x < mapW; x++)
                {
                    for (int l = 0; l < 2; l++)
                    {
                        if (level[x, y, l] != null)
                        {
                            save += compressBlock(blockManager.tiles_Set[level[x, y, l]])+";";
                        }
                        else
                        {
                            save += ";";
                        }
                    }
                }
            }
            save += '\n';
            i++;
        }
        */
        save += '\n'+"img:" + TakeScreenshot();
        File.WriteAllText(Application.dataPath + "/../saves/"+fileName+".smk", save);
        
        ShowDialog("World Saved");
    }
    public void loadFile(string file)
    {
        string[] lines = File.ReadAllLines(Application.dataPath + "/../saves/" + file + ".smk");
        fileName = file;
        fileLabel.text = file;

        if (lines[0].StartsWith("0-"))
        {
            SaveManager.GameMode = lines[0].Replace("0-", "").Replace("\n", "");
            string[] tmp = new string[lines.Length-1];
            Array.Copy(lines, 1, tmp, 0, tmp.Length);
            load_v1_smk(tmp);
        }
        if (lines[0].StartsWith("1-"))
        {
            SaveManager.GameMode = lines[0].Replace("1-", "").Replace("\n", "");
            string[] tmp = new string[lines.Length - 1];
            Array.Copy(lines, 1, tmp, 0, tmp.Length);
            load_v2_smk(tmp);
        }

        reloadLevel();
    }
    private void load_v1_smk(string[] lines)
    {
        level = new List<Level>();

        selectChar(int.Parse(lines[0].Split(';')[0]));
        int lvl = 0;

        for (int i = 1; i < lines.Length; i += 2)
        {
            if (i + 1 < lines.Length)
            {
                var trg = new Level();
                level.Add(trg);
                string[] c = lines[i].Split(';');
                trg.areas[0].night = (c[0] == "n");

                if (c.Length > 1)
                {
                    trg.areas[0].music= (int.Parse(c[1]));
                }
                else
                {
                    trg.areas[0].music = 0;
                }

                trg.areas[0].cell = new placable[mapW, mapH, 2];
                
                int x = 0;
                int y = 0;
                int l = 0;
                bool stop = false;

                foreach (string tile in lines[i + 1].Split(';'))
                {
                    if (!stop)
                    {
                        int block = uncompressBlock(tile);

                        trg.areas[0].cell[x, y, l] = (block > -1) ? blockManager.tiles[block] : null;

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
                
                if (lvl == 0) { reloadLevel(); }
                lvl++;
            }
        }
    }
    private void load_v2_smk(string[] lines)
    {
        level = new List<Level>();

        selectChar(int.Parse(lines[0].Split(';')[0]));

        string save = "";
        int i = 0;

        foreach(string l in lines)
        {
            if (i != 0 && !l.StartsWith("img:"))
                save += l;
            i++;
        }

        level = SaveStat.FromJson(blockManager, save);
    }
    private string compressBlock(int value)
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

    private int uncompressBlock(string value) {
        if (value == "" || value == null)
        {
            return -1;
        }
        int output = 0;

        for (int i = 0; i < value.Length; i++)
        {
            output += (int)(codex.IndexOf(value[i]) * Mathf.Pow(codex.Length, value.Length-i-1));
        }

        return output;
    }

    public void SelectSubArea(int area)
    {
        currentArea = area;
        reloadLevel();
    }

    public string TakeScreenshot()
    {
        Camera camera = screenshotCam;
        camera.backgroundColor = GetComponent<Camera>().backgroundColor;
        RenderTexture rt = new RenderTexture(256, 256, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        
        byte[] bytes = screenShot.EncodeToPNG();
        return Convert.ToBase64String(bytes);
    }

    public byte[] TakeScreenshotWorldIcon()
    {
        Camera camera = screenshotCam;
        camera.backgroundColor = GetComponent<Camera>().backgroundColor;
        RenderTexture rt = new RenderTexture(64, 64, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(64, 64, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 64, 64), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);

        return screenShot.EncodeToPNG();
    }
}
