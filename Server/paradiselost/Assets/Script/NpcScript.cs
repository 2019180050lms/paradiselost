using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcScript : MonoBehaviour
{
    NetworkManager _network;

    public GameObject talkPanel;
    public GameObject questPanel;
    public Text npxtext;
    public Text questtext;
    public Text questIntNum;
    public short _stage;
    int clickCount = 0;

    public MyPlayer player;
    public GameUIManager gameUIManager;
    public static NpcScript Instance { get; } = new NpcScript();

    void Start()
    {
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        talkPanel.SetActive(false);
        questPanel.SetActive(false);
        _stage = 0;
        gameUIManager = GameObject.Find("Game Manager").GetComponent<GameUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(player);
        questIntNum.text = player.questInt.ToString() + "  / 3";
    }

     void OnTriggerStay(Collider other)
    {
        if (other.tag == "MyPlayer")
        {
            MyPlayer obj = other.GetComponent<MyPlayer>();
            Inventory objInven = other.GetComponent<Inventory>();
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (clickCount == 0 && obj.questInt == 0)
                {
                    talkPanel.SetActive(true);
                    npxtext.text = "뒤에 보이는 포탈을 통해 1스테이지에 입장하세요.";
                    clickCount++;
                }
                else if (clickCount == 1 && obj.questInt == 0)
                {
                    npxtext.text = "퀘스트 : 1스테이지의 보스를 처치";
                    clickCount++;
                }
                else if (clickCount == 2 && obj.questInt == 0)
                {
                    npxtext.text = "보상 : 새로운 무기";
                    questtext.text = "1스테이지 보스 처치";
                    player = GameObject.Find("test(Clone)").GetComponent<MyPlayer>();
                    clickCount++;
                }
                else if (clickCount == 3 && obj.questInt == 0)
                {
                    C_Npc npcpacket = new C_Npc();
                    npcpacket.active = true;
                    npcpacket._quest_stage = _stage;

                    //questPanel.SetActive(true);
                    talkPanel.SetActive(false);
                    clickCount = 0;
                    _network.Send(npcpacket.Write());
                }

                // 1스테이지 클리어 이후
                else if(clickCount == 0 && obj.questInt == 1)
                {
                    talkPanel.SetActive(true);
                    npxtext.text = "1스테이지 클리어 보상으로 무기를 지급합니다.";

                    ItemParts reward = GetComponent<ItemParts>();
                    for (int i = 0; i < 9; ++i)
                    {
                        if (objInven.ItemList[i] == null)
                        {
                            objInven.ItemList[i] = Resources.Load<ItemParts>("Items/Weapon Sword Item");
                            gameUIManager.ItemTxt[i].text = "SWORD";
                            break;
                        }
                    }

                    clickCount++;
                }

                else if (clickCount == 1 && obj.questInt == 1)
                {
                    npxtext.text = "새로 생긴 포탈을 통해 2스테이지로 입장하세요";
                    clickCount++;
                }

                else if (clickCount == 2 && obj.questInt == 1)
                {
                    npxtext.text = "퀘스트 : 모든 몬스터 잡기";
                    clickCount++;
                }

                else if (clickCount == 3 && obj.questInt == 1)
                {
                    C_Npc npcpacket = new C_Npc();
                    npcpacket.active = true;
                    npcpacket._quest_stage = _stage;

                    //questPanel.SetActive(true);
                    talkPanel.SetActive(false);
                    clickCount = 0;
                    _network.Send(npcpacket.Write());
                }


                // 2스테이지 이후 
                else if (clickCount == 0 && obj.questInt == 2)
                {
                    talkPanel.SetActive(true);
                    npxtext.text = "이제 마지막 스테이지만 남았습니다.";
                    clickCount++;
                }
                else if (clickCount == 1 && obj.questInt == 2)
                {
                    npxtext.text = "새로 생긴 포탈을 통해 3스테이지에 입장 할 수 있습니다.";
                    clickCount++;
                }
                else if (clickCount == 2 && obj.questInt == 2)
                {
                    npxtext.text = "몬스터들을 처치한 후 마지막 보스까지 처치하십시오.";
                    clickCount++;
                }
                else if (clickCount == 3 && obj.questInt == 2)
                {
                    talkPanel.SetActive(false);
                    clickCount = 0;
                }








                else if (clickCount == 4 && obj.questInt == 2)
                {
                    talkPanel.SetActive(false);
                    clickCount = 0;
                }

                else if (clickCount == 2 && obj.questInt == 2)
                {
                    ItemParts reward = GetComponent<ItemParts>();
                    for (int i = 0; i < 9; ++i)
                    {
                        if (objInven.ItemList[i] == null)
                        {
                            objInven.ItemList[i] = Resources.Load<ItemParts>("Items/Weapon Sword Item");
                            gameUIManager.ItemTxt[i].text = "SWORD";
                            break;
                        }
                    }
                    questPanel.SetActive(false);
                    talkPanel.SetActive(false);
                    clickCount = 0;
                    obj.questInt = 0;
                }
            }

        }
        
    }

}
