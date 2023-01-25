using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    delegate void Keyinput();
    delegate IEnumerator IenumCheck();
    Keyinput keyinput;
    IenumCheck hellocheck;

    private float shoot_time = 0.0f;

    private void Awake()
    {
        keyinput += Check;
        keyinput += Shot;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        keyinput();
        shoot_time += Time.deltaTime;
        
    }

    void Check()
    {
        Debug.Log("hello world");
    }

    void Shot()
    {
        Debug.Log("Shoot");
        Debug.Log(shoot_time);
        if(shoot_time > 2.0f)
        {
            keyinput -= Shot;
        }
    }
}
