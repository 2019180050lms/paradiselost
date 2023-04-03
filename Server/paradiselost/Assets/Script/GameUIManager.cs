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

    public BossEnemy boss;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    void Start()
    {
        Invoke("FindBossHp", 0.5f);
    }

    void FindBossHp()
    {
        boss = GameObject.Find("StageBoss(Clone)").GetComponent<StageBoss>();
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

        bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
    }
}
