using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Anim : MonoBehaviour
{
    Animator testanim;
    // Start is called before the first frame update
    void Start()
    {
        testanim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            testanim.SetTrigger("check1");
        }
        
    }
}
