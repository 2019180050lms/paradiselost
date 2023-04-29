using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sp_Body_Script : MonoBehaviour
{
    public GameObject body;
    public Animator animator;
    void Start()
    {
        animator = body.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Anim_Attack(int num)
    {

    }

    public void Set_Speed(float speed)
    {

    }
}
