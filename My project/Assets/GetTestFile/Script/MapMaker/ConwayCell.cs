using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConwayCell : MonoBehaviour
{
    private Vector2[,] mapcheck = new Vector2[50, 50];
    //0, 0은 -500, -500.
    //50, 50은 500, 500
    //숫자 하나 당 10씩 옮겨짐.

    private GameObject[,] cells = new GameObject[50, 50];

   

    [SerializeField]
    private GameObject cell;

    private float timecheck;

    // Start is called before the first frame update
    void Start()
    {
        timecheck = 0;
        RectTransform rect = GetComponent<RectTransform>();
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                cells[i, j] = Instantiate(cell, new Vector2(rect.rect.width / 50 * i, rect.rect.height / 50 * j), Quaternion.identity, this.transform);
                cells[i, j].SetActive(false);
            }
        }

        cells[25, 24].SetActive(true);
        cells[24, 25].SetActive(true);
        cells[25, 25].SetActive(true);
        cells[26, 25].SetActive(true);
        cells[25, 26].SetActive(true);


    }

   
    

    // Update is called once per frame
    void Update()
    {
        timecheck += Time.deltaTime;
        if (timecheck > 1.0)
        {
            timecheck = 0;
            int[,] cellcount = new int[50, 50];
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    for (int _x = i - 1; _x <= i + 1; _x++)
                    {
                        for (int _y = j - 1; _y <= j + 1; _y++)
                        {
                            if (_x >= 1 && _x <= 49 && _y >= 1 && _y <= 49)
                            {
                                if (cells[_x, _y].activeSelf)
                                {
                                    cellcount[i, j]++;
                                }
                            }
                        }
                    }
                }



            }

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    if (cells[i, j].activeSelf)
                    {
                        if (cellcount[i, j] == 3 || cellcount[i, j] == 4)
                        {
                            cells[i, j].SetActive(true);
                            Debug.Log("live" + i + ", " + j);
                        }
                        else
                        {
                            cells[i, j].SetActive(false);
                        }
                    }
                    else if (!cells[i,j].activeSelf)
                    {
                        if (cellcount[i, j] == 3)
                        {
                            cells[i, j].SetActive(true);
                            Debug.Log("Dead" + i + ", " + j);
                        }
                        else
                        {
                            cells[i, j].SetActive(false);
                        }
                    }
                }
            }
        }
    }

    IEnumerator CenterMake()
    {
        yield return new WaitForSeconds(2.0f);

        RectTransform rect = GetComponent<RectTransform>();
        Debug.Log(rect.rect.width);
        
        cells[25, 25] = Instantiate(cell, new Vector2(rect.rect.width/2, rect.rect.height/2), Quaternion.identity, this.transform);
        Debug.Log("birth");
    }
}
