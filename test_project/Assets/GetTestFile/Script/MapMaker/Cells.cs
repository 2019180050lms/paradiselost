using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cells : MonoBehaviour
{
    public int x;
    public int y;
    public bool alive;
    // Start is called before the first frame update

    CellGrid cellgrid;
    Image image;

    private void Awake()
    {
        cellgrid = transform.parent.GetComponent<CellGrid>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        image.color = alive ? Color.white : new Color(0.2f, 0.2f, 0.2f);
    }

    public void Toggle()
    {
        if(cellgrid.Playing)
        {
            return;
        }
        alive = !alive;
        cellgrid.Grid[x, y] = alive;
    }

}
