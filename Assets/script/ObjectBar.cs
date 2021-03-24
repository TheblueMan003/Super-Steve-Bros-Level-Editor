using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ObjectBar : MonoBehaviour
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
        if (cont.lastBlocks[index] != null)
        {
            img.sprite = cont.lastBlocks[index].icon;
        }
        coolDown -= Time.deltaTime;
    }

    public void Click()
    {
        if (coolDown <= 0)
        {
            var tmp = cont.lastBlocks[index];
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
