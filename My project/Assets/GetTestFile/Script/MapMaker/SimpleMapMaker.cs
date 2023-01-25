using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SimpleMapMaker : MonoBehaviour
{
    private int CaveMaxCount = 12;

    public GameObject Cube;
    

    [SerializeField] private GameObject centercheckcube;
    [SerializeField] private GameObject Wood;


    public GameObject[,] hellocube;
    public GameObject[] centercubes;
    public Camera maincamera;
    public Camera subcamera;
    

    public float Maketime;
    public RectInt rect;
    // Start is called before the first frame update
    float timecheck = 0.0f;

    


    private void Awake()
    {
        maincamera.enabled = true;
        hellocube = new GameObject[3, 3];
        centercubes = new GameObject[CaveMaxCount];
        
        rect.x = 0;
        rect.y = 0;
    }
    void Start()
    {
        //StartCoroutine(MakeCube());
        while (true)
        {

            hellocube[rect.x, rect.y] = Instantiate(Cube);
            if(rect.x %2 == 0 && rect.y%2 == 0)
            {
                GameObject tree = Instantiate(Wood);
                tree.transform.parent = hellocube[rect.x, rect.y].transform;
            }
            hellocube[rect.x, rect.y].transform.position = new Vector3(rect.x * 100.0f, 0, rect.y * 100.0f);
            if (rect.x < 2)
            {
                rect.x++;
            }
            else
            {
                rect.x = 0;
                rect.y++;
            }

            


            if (rect.y >= 3)
            {
                rect.x = 0;
                rect.y = 0;

                break;
            }

            
        }
        int mapcount = 0;

        for(int x = 0;x<3; x++)
        {
            for(int y = 0;y<3;y++)
            {
                if(x+1 < 3)
                {
                    if (mapcount >= CaveMaxCount)
                    {
                        break;
                    }
                    centercubes[mapcount] = Instantiate(centercheckcube);
                    centercubes[mapcount].transform.position = Vector3.Lerp(hellocube[x, y].transform.position, hellocube[x + 1, y].transform.position, 0.5f);
                    mapcount++;
                   
                }
                if (y + 1 < 3)
                {
                    if (mapcount >= CaveMaxCount)
                    {
                        break;
                    }
                    centercubes[mapcount] = Instantiate(centercheckcube);
                    centercubes[mapcount].transform.position = Vector3.Lerp(hellocube[x, y].transform.position, hellocube[x, y + 1].transform.position, 0.5f);
                    mapcount++;
                }
            }
        }

        

    }

    private void FixedUpdate()
    {
       

        Maketime += Time.deltaTime;
        timecheck += Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space");




            maincamera.enabled = !maincamera.enabled;
            if (!maincamera.gameObject.activeSelf)
            {
                int camera_x = Random.Range(0, 3);
                int camera_y = Random.Range(0, 3);
                subcamera = hellocube[camera_x, camera_y].transform.Find("Camera").gameObject.GetComponent<Camera>();
                subcamera.enabled = true;
            }
            else
            {
                subcamera.enabled = false;
            }


        }

       
        
    
    }


    IEnumerator MakeCube()
    {
        while (true)
        {
            Maketime = 0f;
            yield return new WaitUntil(() => Maketime >= 1.0f);
            

            hellocube[rect.x, rect.y] = Instantiate(Cube);
            hellocube[rect.x, rect.y].transform.position = new Vector3(rect.x * 100.0f, 0, rect.y * 100.0f) ;
            if (rect.x < 2)
            {
                rect.x++;
            }
            else
            {
                rect.x = 0;
                rect.y++;
            }


            if(rect.y>=3)
            {
                rect.x = 0;
                rect.y = 0;

                break;
            }
        }
    }
}
