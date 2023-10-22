using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sp_Leg_Script : MonoBehaviour
{
    public GameObject leg;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = leg.GetComponent<Animator>();
    }
    public void Set_Speed(float speed)
    {
        animator.SetFloat("speed", 10.0f);
       
    }
    public void Set_Pos(int num)
    {

        animator.SetBool("IsFight", true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Fight_check()
    {
        yield return new WaitForSeconds(2);
        animator.SetBool("IsFight", false);
    }
}
