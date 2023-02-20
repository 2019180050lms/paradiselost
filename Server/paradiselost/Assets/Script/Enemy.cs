using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyId;
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxcollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxcollider = GetComponent<BoxCollider>();

    }


}
