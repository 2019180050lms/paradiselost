using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    NetworkManager _network;

    MyPlayer myPlayer;
    Inventory inventory;
    public float playTime;

    BoxCollider boxCollider;

    public Text stageTxt;
    public Text playerHealthTxt;
    public Text playTimeTxt;

    
    public int hP = 0;
    int index = 0;

    public BossEnemy boss;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    public RectTransform playerHealthGroup;
    public RectTransform playerHealthBar;

    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;

    public List<Image> ItemImg = new List<Image>() { };
    //public List<Button> buttonList = new List<Button>();
    public GameObject BossUI;
    public GameObject PlayerUI;

    public List<Text> ItemTxt = new List<Text>() { };

    public List<Text> otherPlayerHPText = new List<Text>() { };

    public GameObject[] playersUI;
    public GameObject[] playersUIImages;
    //public List<RectTransform> otherPlayersHealthGroup = new List<RectTransform>() { };
    public List<RectTransform> otherPlayersHealthBar = new List<RectTransform>() { };

    void Start()
    {
        Invoke("FindMyPlayer", 0.5f);
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        DontDestroyOnLoad(this);
    }

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();


    }

    void FindBossHp()
    {
        boss = GameObject.Find("BossTest(Clone)").GetComponent<BossEnemy>();
    }

    void FindMyPlayer()
    {

        inventory = GameObject.FindGameObjectWithTag("MyPlayer").GetComponent<Inventory>();
        myPlayer = GameObject.FindGameObjectWithTag("MyPlayer").GetComponent<MyPlayer>();
    }
    public void FindPlayerUI()
    {
        playersUI = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        playTime += Time.deltaTime;
        //playerHealthTxt.text = MyPlayer.health.ToString();
    }

    void LateUpdate()
    {

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
        playerHealthTxt.text = myPlayer.hp.ToString() + " /  100";  // 플레이어 체력 표시

        playerHealthBar.localScale = new Vector3((float)PlayerManager.Instance._myplayer.hp / 100, 1, 1);

        if(boss != null)
        {
            Debug.Log("boss != null");
            bossHealthBar.localScale = new Vector3((float)PlayerManager.Instance._boss.curHealth / boss.maxHealth, 1, 1);
        }

        if (playersUI != null)
        {
            for (int i = 0; i < playersUI.Length; ++i)
            {
                playersUIImages[i].SetActive(true);
                short otherPlayerHP = playersUI[i].GetComponent<Player>().hp;
                otherPlayerHPText[i].text = otherPlayerHP.ToString() + " /  100";
                otherPlayersHealthBar[i].localScale = new Vector3((float)otherPlayerHP / 100, 1, 1);
            }
        }

        if (inventory.ItemList[0] != null)
        {
            for (int i = 0; i < 9; ++i)
            {
                if (inventory.ItemList[i].hasItem)
                {
                    ItemImg[i].color = new Color(1, 1, 1, 1);
                }
                else if (inventory.ItemList[i] == null)
                    ItemImg[i].color = new Color(1, 1, 1, 0);
            }
        }
       

        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "MyPlayer")
        {
            Debug.Log("Enter BossRoom");
            FindBossHp();
            BossUI.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag =="MyPlayer")
        {
            Debug.Log("Enter BossRoom");
            FindBossHp();
            BossUI.SetActive(true);
        }
    }

    public void FindTagName(string TagName)  // 태그로 몇번 칸인지 검색
    {
        if (TagName == "Inven1")
            index = 0;
        else if (TagName == "Inven2")
            index = 1;
        else if (TagName == "Inven3")
            index = 2;
        else if (TagName == "Inven4")
            index = 3;
        else if (TagName == "Inven5")
            index = 4;
        else if (TagName == "Inven6")
            index = 5;
        else if (TagName == "Inven7")
            index = 6;
        else if (TagName == "Inven8")
            index = 7;
        else if (TagName == "Inven9")
            index = 8;
    }

    void C_Send_Item(ushort itemType, ushort itemNum)
    {
        C_Item item_packet = new C_Item();
        item_packet.playerId = PlayerManager.Instance._myplayer.PlayerId;
        item_packet.charactorType = itemType;
        item_packet.itemType = itemNum;
        _network.Send(item_packet.Write());
    }

    public void OnClick()
    {
        string tagName = EventSystem.current.currentSelectedGameObject.tag;
        FindTagName(tagName);


        if(inventory.ItemList[index].tag == "Head_Item") // 해당 칸의 아이템이 머리 파츠이면
        {
            C_Send_Item(1, (ushort)inventory.ItemList[index].type);
            ItemImg[index].color = new Color(1, 1, 1, 0); // 장비 아이콘 UI 끄기
            //inventory.ItemList.RemoveAt(index); // 인벤 List에서 해당 파츠 제거
            inventory.ItemList[index] = null;
            ItemTxt[index].text = " ";
            
        }
        else if (inventory.ItemList[index].tag == "Body_Item") 
        {
            C_Send_Item(2, (ushort)inventory.ItemList[index].type);
            ItemImg[index].color = new Color(1, 1, 1, 0);
            inventory.ItemList[index] = null;
            ItemTxt[index].text = " ";
        }
        else if (inventory.ItemList[index].tag == "Leg_Item")
        {
            C_Send_Item(3, (ushort)inventory.ItemList[index].type);
            ItemImg[index].color = new Color(1, 1, 1, 0);
            //inventory.ItemList.RemoveAt(index);
            inventory.ItemList[index] = null;
            ItemTxt[index].text = " ";
        }
        else if(inventory.ItemList[index].tag == "Weapon")
        {
            C_Send_Item(2, (ushort)inventory.ItemList[index].value);
            myPlayer.hasWeapons[0] = false;
            myPlayer.hasWeapons[1] = false;
            myPlayer.hasWeapons[2] = false;
            myPlayer.hasWeapons[3] = false;
            myPlayer.hasWeapons[inventory.ItemList[index].value] = true;
            //myPlayer.equipWeapon = myPlayer.weapons[0].GetComponent<Weapon>();
            //myPlayer.equipWeapon.gameObject.SetActive(true);
            ItemImg[index].color = new Color(1, 1, 1, 0);
            inventory.ItemList[index] = null;
            ItemTxt[index].text = " ";
        }
        else if (inventory.ItemList[index].tag == "Potion")
        {
            //C_Send_Item(2, (ushort)inventory.ItemList[index].value);
            myPlayer.hp += 10;
            
            ItemImg[index].color = new Color(1, 1, 1, 0);
            inventory.ItemList[index] = null;
            ItemTxt[index].text = " ";
        }

        //for (int i = 0; i < 9; ++i) // 인벤 정렬
        //{
        //  if (inventory.ItemList[i] == null)
        //        ItemImg[i].color = new Color(1, 1, 1, 0);
        //}

    }
}