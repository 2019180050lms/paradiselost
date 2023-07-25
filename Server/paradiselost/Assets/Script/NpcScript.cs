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
                    npxtext.text = "111";
                    clickCount++;
                }
                else if (clickCount == 1 && obj.questInt == 0)
                {
                    npxtext.text = "����Ʈ : ���� 3���� ���";
                    clickCount++;
                }
                else if (clickCount == 2 && obj.questInt == 0)
                {
                    npxtext.text = "���� : Į";
                    questtext.text = "���� 3���� ���";
                    player = GameObject.Find("test(Clone)").GetComponent<MyPlayer>();
                    clickCount++;
                }
                else if (clickCount == 3 && obj.questInt == 0)
                {
                    C_Npc npcpacket = new C_Npc();
                    npcpacket.active = true;
                    npcpacket._quest_stage = _stage;

                    questPanel.SetActive(true);
                    talkPanel.SetActive(false);
                    clickCount = 0;
                    _network.Send(npcpacket.Write());
                }

                else if(clickCount == 0 && obj.questInt > 0 && obj.questInt < 3)
                {
                    talkPanel.SetActive(true);
                    npxtext.text = "����Ʈ�� ���� �Ϸ����� �ʾҽ��ϴ�.";
                    clickCount++;
                }

                else if (clickCount == 1 && obj.questInt > 0 && obj.questInt < 3)
                {
                    talkPanel.SetActive(false);
                    clickCount = 0;
                }

                else if (clickCount == 0 && obj.questInt > 0 && obj.questInt == 3)
                {
                    talkPanel.SetActive(true);
                    npxtext.text = "����Ʈ�� �Ϸ��߽��ϴ�.";
                    clickCount++;
                }

                else if (clickCount == 1 && obj.questInt == 3)
                {
                    npxtext.text = "�������� ���� �����մϴ�.";
                    _stage++;
                    clickCount++;
                }

                else if (clickCount == 2 && obj.questInt == 3)
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
