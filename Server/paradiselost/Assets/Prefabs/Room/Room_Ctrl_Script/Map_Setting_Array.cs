using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Setting_Array : MonoBehaviour
{
    int[] array_check = new int[10] { 4, 2, 2, 2, 4, 2, 2, 4, 2, 4 };
    public GameObject ternal_LR;
    public GameObject dungeon;

    private float setting_x;
    private float setting_y;
    private int _Last;


    // Start is called before the first frame update
    void Start()
    {
        setting_x = 0;
        setting_y = 0;
        _Last = 0;
        CreateDungeon();
    }

    void CreateDungeon()
    {
        foreach(int i in array_check)
        {
            switch(i)
            {
                case 2:
                    if(_Last == 4)
                    {
                        setting_x += 8.5f;
                    }
                    else
                    {
                        setting_x += 17;
                    }
                    Instantiate(ternal_LR, new Vector3(setting_x, setting_y, 0), Quaternion.identity);
                    _Last = 2;
                    break;
                case 4:
                    if(_Last == 2)
                    {
                        setting_x += 30;
                    }
                    Instantiate(dungeon, new Vector3(setting_x, setting_y, 0), Quaternion.identity);
                    setting_x += 30;

                    _Last = 4;
                    break;
            }
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
