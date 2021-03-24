using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMenu : MonoBehaviour
{
    public int index;
    private RayController cont;
    public float coolDown = 0;

    void Start()
    {
        cont = GameObject.FindObjectOfType<RayController>();
    }
    void Update()
    {
        coolDown -= Time.deltaTime;
    }

    public void Click()
    {
        if (coolDown <= 0)
        {
            cont.isMenuOpen = !cont.isMenuOpen;
            cont.isSaveOpen = false;
        }
    }
}
