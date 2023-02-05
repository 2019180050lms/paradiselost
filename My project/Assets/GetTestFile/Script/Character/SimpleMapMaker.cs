using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SimpleMapMaker : MonoBehaviour
{
    [SerializeField] private GameObject Redplate;
    [SerializeField] private GameObject Blackplate;
    [SerializeField] private GameObject Grayplate;

    public GameObject[,] hellocube;
    public GameObject play_statue;
    public GameObject object_object;
    public GameObject monster;


    int count;
    

    public float Maketime;
    public RectInt rect;
    // Start is called before the first frame update
    float timecheck = 0.0f;

    


    private void Awake()
    {
        count = 5;

       
        hellocube = new GameObject[33, 33]; // 2 *8 + 1
        
        rect.x = 0;
        rect.y = 0;
       for(int i = 0;i< 33;i++)
        {
            for(int j = 0;j<33;j++)
            {
                if (i >= 15 && i <= 17)
                {
                    if (j == 0 || j == 1 || j == 31 || j == 32)
                    {
                        hellocube[i,j] =  Instantiate(Blackplate, new Vector3(i * 10.0f, 0, j * 10.0f), Quaternion.identity, this.transform);
                        hellocube[i, j].gameObject.name = "Blackplate";
                        

                        if(Random.Range(0, 10) == 0 && count > 0)
                        {
                            Instantiate(play_statue, hellocube[i,j].transform.position, Quaternion.identity, this.transform);
                            count--;
                        }
                    }
                    else if (j >= 15 && j <= 17)
                    {
                        hellocube[i, j] = Instantiate(Grayplate, new Vector3(i * 10.0f, 0, j * 10.0f), Quaternion.identity, this.transform);
                        if(count > 2)
                        {
                            Instantiate(monster, hellocube[i, j].transform.position, Quaternion.identity, this.transform);
                            count--;
                        }
                    }
                    else
                    {
                        hellocube[i, j] = Instantiate(Redplate, new Vector3(i * 10.0f, 0, j * 10.0f), Quaternion.identity, this.transform);
                    }
                }
                else if (i == 0 || i == 1 || i == 31 || i == 32)
                {
                    if (j >= 15 && j <= 17)
                    {
                        hellocube[i, j] = Instantiate(Blackplate, new Vector3(i * 10.0f, 0, j * 10.0f), Quaternion.identity, this.transform);
                        if (Random.Range(0, 50) == 0 && count > 0)
                        {
                            Instantiate(play_statue, hellocube[i, j].transform.position + new Vector3(0, 10, 0), Quaternion.identity, this.transform);
                        }
                    }
                    else
                    {
                        hellocube[i, j] = Instantiate(Redplate, new Vector3(i * 10.0f, 0, j * 10.0f), Quaternion.identity, this.transform);
                    }
                }
                else
                {
                    hellocube[i, j] = Instantiate(Redplate, new Vector3(i * 10.0f, 0, j * 10.0f), Quaternion.identity, this.transform);
                }
            }
        }

    }
    void Start()
    {
        //StartCoroutine(MakeCube());
       

       

        

    }

    private void FixedUpdate()
    {
       

        Maketime += Time.deltaTime;
        timecheck += Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
      

       
        
    
    }


    IEnumerator MakeCube()
    {
        while (true)
        {
            Maketime = 0f;
            yield return new WaitUntil(() => Maketime >= 1.0f);
            

            
          

          
        }
    }


   
}
