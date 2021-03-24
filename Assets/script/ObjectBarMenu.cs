using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectBarMenu : MonoBehaviour
{
    public int index;
    public Image img;
    public Image img2;
    private RayController cont;
    public float coolDown = 0;
    void Start()
    {
        cont = GameObject.FindObjectOfType<RayController>();
        img2 = GetComponent<Image>();
    }

    void Update()
    {
        if (cont.blockLst[cont.currentMenu].item.Length > index && cont.blockLst[cont.currentMenu].item[index] != null)
        {
            img.sprite = cont.blockLst[cont.currentMenu].item[index].icon;
            img.color = new Color(1, 1, 1, 1);
            img2.color = new Color(1, 1, 1, 1);
        }
        else
        {
            img.sprite = null;
            img.color = new Color(1, 1, 1, 0);
            img2.color = new Color(1, 1, 1, 0);
        }
        coolDown -= Time.deltaTime;
    }

    public void Click()
    {
        if (coolDown <= 0)
        {
            var tmp = cont.blockLst[cont.currentMenu].item[index];
            cont.currentBlock = tmp;
            cont.lastBlocks.Remove(tmp);
            cont.lastBlocks.Insert(0, tmp);
            if (cont.lastBlocks.Count > 10)
            {
                cont.lastBlocks.RemoveAt(10);
            }
        }
    }
}
