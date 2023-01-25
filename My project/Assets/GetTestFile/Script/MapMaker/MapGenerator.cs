using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int mapsize;
    [SerializeField] private GameObject map;
    // Start is called before the first frame update
    void Start()
    {
        DrawMap(0, 0);
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DrawMap(int x, int y)
    {
        LineRenderer linerenderer = Instantiate(map).GetComponent<LineRenderer>();
        linerenderer.SetPosition(0, new Vector2(x, y) - mapsize / 2);
        linerenderer.SetPosition(1, new Vector2(x + mapsize.x, y) - mapsize / 2);
        linerenderer.SetPosition(2, new Vector2(x + mapsize.x, y + mapsize.y) - mapsize / 2);
        linerenderer.SetPosition(3, new Vector2(x, y + mapsize.y) - mapsize / 2);
    }
}
