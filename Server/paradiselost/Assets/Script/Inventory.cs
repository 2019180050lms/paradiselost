using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    //public ItemParts[] ItemList = new ItemParts[9];

    public List<ItemParts> ItemList = new List<ItemParts>() { };

    public GameObject InventoryUI;

    public GameUIManager gameUIManager;

    bool I_key;

    public bool hasHeadItem;
    void Start()
    {
        Invoke("FindInven", 1f);
    }
    void FindInven()
    {
        Debug.Log("FindInven");
        InventoryUI = GameObject.FindGameObjectWithTag("InventoryUI");
        gameUIManager = GameObject.Find("Game Manager").GetComponent<GameUIManager>();
        InventoryUI.SetActive(false);
        //weapon1Img = Image.
    }
    void Update()
    {
        if (!I_key && Input.GetButtonDown("Inventory"))
        {
            I_key = true;
            InventoryUI.SetActive(true);
            Debug.Log("아이템 창 키기");
        }

        else if( I_key && Input.GetButtonDown("Inventory"))
        {
            I_key = false;
            InventoryUI.SetActive(false);
            Debug.Log("아이템 창 끄기");
        }
        //Debug.Log(ItemList[0]);
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (other.tag == "Head_Item")
            {
                ItemParts obj = other.GetComponent<ItemParts>();
                for (int i = 0; i < 9; ++i)
                {
                    if (ItemList[i] == null)
                    {
                        if (obj.type == 1)
                        {
                            ItemList[i] = Resources.Load<ItemParts>("Items/Po_Head_Item");
                            gameUIManager.ItemTxt[i].text = "Power head";
                        }
                        else if (obj.type == 2)
                        {
                            ItemList[i] = Resources.Load<ItemParts>("Items/Sh_Head_Item");
                            gameUIManager.ItemTxt[i].text = "Shield head";
                        }
                        else if (obj.type == 3)
                        {
                            ItemList[i] = Resources.Load<ItemParts>("Items/Sp_Head_Item");
                            gameUIManager.ItemTxt[i].text = "Speed head";
                        }
                        break;
                    }
                }
                Destroy(other.gameObject);
                PlayerManager.Instance.item = null;
            }
            else if (other.tag == "Weapon")
            {
                ItemParts obj = other.GetComponent<ItemParts>();
                for (int i = 0; i < 9; ++i)
                {
                    if (ItemList[i] == null)
                    {
                        if(obj.value == 0)
                        {
                            ItemList[i] = Resources.Load<ItemParts>("Items/Weapon Sword Item");
                            gameUIManager.ItemTxt[i].text = "SWORD";
                            Destroy(other.gameObject);
                            break;
                        }
                        else if(obj.value == 1)
                        {
                            ItemList[i] = Resources.Load<ItemParts>("Items/Weapon Rifle Item");
                            gameUIManager.ItemTxt[i].text = "RIFLE";
                            Destroy(other.gameObject);
                            break;
                        }
                        else if (obj.value == 2)
                        {
                            ItemList[i] = Resources.Load<ItemParts>("Items/Weapon 2H Sword Item");
                            //gameUIManager.ItemTxt[i].text = "2H Sword";
                            gameUIManager.ItemImg[i].sprite = Resources.Load<Sprite>("Items/2HSword");
                            Destroy(other.gameObject);
                            break;
                        }
                        else if (obj.value == 3)
                        {
                            ItemList[i] = Resources.Load<ItemParts>("Items/Weapon ShotGun Item ");
                            gameUIManager.ItemImg[i].sprite = Resources.Load<Sprite>("Items/shotgun");
                            //gameUIManager.ItemTxt[i].text = "Shot Gun";
                            Destroy(other.gameObject);
                            break;
                        }
                    }
                }
                
            }
            else if (other.tag == "Potion")
            {
                ItemParts obj = other.GetComponent<ItemParts>();
                for (int i = 0; i < 9; ++i)
                {
                    if (ItemList[i] == null)
                    {
                        if (obj.value == 10)
                        {
                            ItemList[i] = Resources.Load<ItemParts>("Items/HP Potion");
                            //gameUIManager.ItemTxt[i].text = "HP Potion";
                            gameUIManager.ItemImg[i].sprite = Resources.Load<Sprite>("Items/potion");
                            Destroy(other.gameObject);
                            break;
                        }
                    }
                }

            }
        }
    }

}
