using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    MyPlayer myPlayer;
    Joint_Robot joint_robot;
    public float playTime;

    BoxCollider boxCollider;

    public Text stageTxt;
    public Text playerHealthTxt;
    public Text playTimeTxt;

    
    public int hP = 0;

    public BossEnemy boss;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;

    public GameObject BossUI;
    void Start()
    {
        Invoke("ItemIcon", 0.5f);
    }

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        
        
        weapon2Img.color = new Color(0, 0, 0, 0);


    }

    void FindBossHp()
    {
        boss = GameObject.Find("StageBoss(Clone)").GetComponent<StageBoss>();
    }

    void ItemIcon()
    {

        myPlayer = GameObject.FindGameObjectWithTag("MyPlayer").GetComponent<MyPlayer>();

        if (myPlayer.hasHeadItem)
        {
            Debug.Log("dasd");
            //weapon1Img.color = new Color(1, 1, 1, 0);
        }
    }

     void Update()
    {
        playTime += Time.deltaTime;
        //playerHealthTxt.text = MyPlayer.health.ToString();

        Debug.Log(myPlayer.hasHeadItem);

        if (myPlayer.hasHeadItem == true)
        {
            weapon1Img.color = new Color(1, 1, 1, 0);
        }
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

        

        bossHealthBar.localScale = new Vector3((float)PlayerManager.Instance._boss.curHealth / boss.maxHealth, 1, 1);

        //weapon1Img.color = new Color(1, 1, 1, myPlayer.hasHeadItem ? 1 : 0);

        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Enter BossRoom");
            FindBossHp();
            BossUI.SetActive(true);
        }
    }
}
