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

    public GameObject portal1;
    public GameObject portal2;
    public GameObject portal3;

    public MyPlayer player;
    public GameUIManager gameUIManager;
    public static NpcScript Instance { get; } = new NpcScript();

    void Start()
    {
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        talkPanel.SetActive(false);
        questPanel.SetActive(false);

        portal1.SetActive(false);
        portal2.SetActive(false);
        portal3.SetActive(false);

        _stage = 0;
        gameUIManager = GameObject.Find("Game Manager").GetComponent<GameUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(player);
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
                    npxtext.text = "�ڿ� ���̴� ��Ż�� ���� 1���������� �����ϼ���.";
                    clickCount++;
                }
                else if (clickCount == 1 && obj.questInt == 0)
                {
                    npxtext.text = "����Ʈ : 1���������� ������ óġ";
                    clickCount++;
                }
                else if (clickCount == 2 && obj.questInt == 0)
                {
                    npxtext.text = "���� : ���ο� ����";
                    questtext.text = "1�������� ���� óġ";
                    clickCount++;
                    // player = GameObject.Find("test(Clone)").GetComponent<MyPlayer>();
                }

                else if (clickCount == 3 && obj.questInt == 0)
                {
                    C_Npc npcpacket = new C_Npc();
                    npcpacket.active = true;
                    npcpacket._quest_stage = _stage;

                    //questPanel.SetActive(true);
                    talkPanel.SetActive(false);
                    portal1.SetActive(true);
                    clickCount = 0;
                    _network.Send(npcpacket.Write());
                }

                // 1�������� Ŭ���� ����
                else if (clickCount == 0 && obj.questInt == 1)
                {
                    talkPanel.SetActive(true);
                    npxtext.text = "1�������� Ŭ���� �������� ���⸦ �����մϴ�.";

                    ItemParts reward = GetComponent<ItemParts>();
                    for (int i = 0; i < 9; ++i)
                    {
                        if (objInven.ItemList[i] == null)
                        {
                            if (obj.playerType == 1)
                            {
                                objInven.ItemList[i] = Resources.Load<ItemParts>("Items/Weapon 2H Sword Item");
                                gameUIManager.ItemTxt[i].text = "2H SWORD";
                                break;
                            }
                            else if (obj.playerType == 0)
                            {
                                objInven.ItemList[i] = Resources.Load<ItemParts>("Items/Weapon ShotGun Item");
                                gameUIManager.ItemTxt[i].text = "SHOT GUN";
                                break;
                            }
                        }
                    }

                    clickCount++;
                }

                else if (clickCount == 1 && obj.questInt == 1)
                {
                    npxtext.text = "���� ���� ��Ż�� ���� 2���������� �����ϼ���";
                    clickCount++;
                }

                else if (clickCount == 2 && obj.questInt == 1)
                {
                    npxtext.text = "����Ʈ : ���͸� ���� ���� ã��";
                    clickCount++;
                }

                else if (clickCount == 3 && obj.questInt == 1)
                {
                    C_Npc npcpacket = new C_Npc();
                    npcpacket.active = true;
                    npcpacket._quest_stage = _stage;

                    //questPanel.SetActive(true);
                    portal2.SetActive(true);
                    talkPanel.SetActive(false);
                    clickCount = 0;
                    _network.Send(npcpacket.Write());
                }


                // 2�������� ���� 
                else if (clickCount == 0 && obj.questInt > 1)
                {
                    talkPanel.SetActive(true);
                    npxtext.text = "���� ������ ���������� ���ҽ��ϴ�.";
                    clickCount++;
                }
                else if (clickCount == 1 && obj.questInt > 1)
                {
                    npxtext.text = "���� ���� ��Ż�� ���� 3���������� ���� �� �� �ֽ��ϴ�.";
                    clickCount++;
                }
                else if (clickCount == 2 && obj.questInt > 1)
                {
                    npxtext.text = "���͵��� óġ�� �� ������ �������� óġ�Ͻʽÿ�.";
                    clickCount++;
                }
                else if (clickCount == 3 && obj.questInt > 1)
                {
                    portal3.SetActive(true);
                    talkPanel.SetActive(false);
                    clickCount = 0;
                }








                //else if (clickCount == 4 && obj.questInt == 2)
                //{
                //    talkPanel.SetActive(false);
                //    clickCount = 0;
                //}

                //else if (clickCount == 2 && obj.questInt == 2)
                //{
                //    ItemParts reward = GetComponent<ItemParts>();
                //    for (int i = 0; i < 9; ++i)
                //    {
                //        if (objInven.ItemList[i] == null)
                //        {
                //            if(obj.playerType == 1)
                //            {
                //                objInven.ItemList[i] = Resources.Load<ItemParts>("Items/Weapon 2H Sword Item");
                //                gameUIManager.ItemTxt[i].text = "2H SWORD";
                //                break;
                //            }
                //            else
                //            {
                //                objInven.ItemList[i] = Resources.Load<ItemParts>("Items/Weapon ShotGun Item ");
                //                gameUIManager.ItemTxt[i].text = "SHOT GUN";
                //                break;
                //            }
                //        }
                //    }
                //    questPanel.SetActive(false);
                //    talkPanel.SetActive(false);
                //    clickCount = 0;
                //    obj.questInt = 0;
                //}
            }

        }
        
    }

     void OnTriggerExit(Collider other)
    {
        talkPanel.SetActive(false);
    }

}
