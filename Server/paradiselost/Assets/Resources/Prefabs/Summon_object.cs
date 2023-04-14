using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon_object : MonoBehaviour
{
    public GameObject[] player_model = new GameObject[3];
    public Animator[] player_animator = new Animator[3];

    public bool is_punch_moving;

    // Start is called before the first frame update
    void Start()
    {
        is_punch_moving = false;
        player_model[0] = Instantiate(Resources.Load<GameObject>("Prefabs/Player/Sp/Sp_Body_Parts"), this.transform);
        player_animator[0] = player_model[0].gameObject.transform.GetChild(0).GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!is_punch_moving)
        {
            StartCoroutine(Fist_gun());
        }
       
            
        

       
    }

    IEnumerator Fist_gun()
    {
        is_punch_moving = true;
        player_animator[0].SetBool("IsFight", true);
        yield return new WaitUntil(() => player_animator[0].GetCurrentAnimatorStateInfo(0).IsName("fist_gun 0") &&
            player_animator[0].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        player_animator[0].SetBool("IsFight", false);
        is_punch_moving = false;
    }
   
}
