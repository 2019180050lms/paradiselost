using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Shield_Player : MonoBehaviour
{
    public GameObject head;
    public GameObject body;
    public GameObject leg;

    public Animator head_anim;
    public Animator body_anim;
    public Animator leg_anim;
    // Start is called before the first frame update
    void Start()
    {
        body_anim = body.transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            body_anim.SetTrigger("shield_pos");
            body_anim.SetBool("Stop", true);
            body_anim.SetBool("Shield", true);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            body_anim.SetBool("Stop", false);
            body_anim.SetFloat("Speed", 10.0f);
        }

    }


}
