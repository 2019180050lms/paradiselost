using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Robot_Default : MonoBehaviour, IDAMAGE
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Damage(float damage)
    {

    }

    public void Heal(float heal)
    {

    }

    // Update is called once per frame
    void Update()
    {
        IDAMAGE damage = GetComponent<IDAMAGE>();

        if(damage == null)
        {
            Debug.Log("hello world");
        }
        else
        {
            Debug.Log("into_interface");
        }
    }
}
