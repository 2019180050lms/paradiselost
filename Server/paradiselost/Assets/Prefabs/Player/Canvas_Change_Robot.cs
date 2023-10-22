using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas_Change_Robot : MonoBehaviour
{
    public static Canvas_Change_Robot Instance { get; } = new Canvas_Change_Robot();
    public Joint_Robot joint;
    public int checkNum;
    // Start is called before the first frame update
    void Start()
    {
        checkNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        checkNum = 0;
    }

    public void Check_Po()
    {
        Debug.Log("PO");
        checkNum = 1;
    }

    public void Check_Sh()
    {
        Debug.Log("SH");
        checkNum = 2;
    }

    public void Check_Sp()
    {
        Debug.Log("SP");
        checkNum = 3;
    }
}
