using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBar : MonoBehaviour
{
    public int index;
    public Image img;
    private RayController cont;
    public float coolDown = 0;

    void Start()
    {
        cont = GameObject.FindObjectOfType<RayController>();
    }
    void Update()
    {
        coolDown -= Time.deltaTime;
        img.sprite = cont.blockLst[index].item[0].icon;
    }

    public void Click()
    {
        if (coolDown <= 0)
        {
            cont.currentMenu = index;
        }
    }
}
