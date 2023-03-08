using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    MyPlayer myPlayer;
    public float playTime;


    public Text stageTxt;
    public Text playerHealthTxt;
    public Text playTimeTxt;

    public int hP = 0;
    void Start()
    {

    }

     void Update()
    {
        playTime += Time.deltaTime;
        //playerHealthTxt.text = MyPlayer.health.ToString();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //MyPlayer myplayer = GameObject.Find("Player").GetComponent<MyPlayer>();

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);


        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
        playerHealthTxt.text = MyPlayer.health.ToString() + " /  100"; 
    }
}
