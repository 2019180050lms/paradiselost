using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSetting : MonoBehaviour
{
    private Transform transform2;


    public List<GameObject> objects = new List<GameObject>();
    public GameObject monster;

    // Start is called before the first frame update
    void Start()
    {
        int maxcount = Random.Range(5, 9);

        transform2 = gameObject.transform.GetChild(0).GetComponent<Transform>();
        Debug.Log(transform.localScale.x + ", " + transform.localScale.y);

        float x = transform2.localScale.x;
        float y = transform2.localScale.y;

        

        for(float i = 0.0f;i<x;i+=50.0f)
        {
            for (float j = 0.0f; j < y; j += 50.0f)
            {
                int randomcheck = Random.Range(0, 10);
                if (i < x / 2.0f + 10.0f && i > x / 2.0f - 10.0f)
                {
                    if(j < y / 2.0f + 10.0f && j > y / 2.0f - 10.0f)
                    {
                        if(maxcount > 0)
                        {
                            Instantiate(monster, new Vector3(i, 0, j), Quaternion.identity, gameObject.transform);
                            maxcount--;
                        }
                    }
                }
                else
                {
                    if(randomcheck < objects.Count)
                    {
                        Instantiate(objects[randomcheck], new Vector3(i, 0, j), Quaternion.identity, gameObject.transform);
                        maxcount--;
                    }
                }

                if(maxcount <= 0)
                {
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
