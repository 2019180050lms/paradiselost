using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    //public ItemParts[] ItemList = new ItemParts[9];

    public List<ItemParts> ItemList = new List<ItemParts>() { };

    public GameObject InventoryUI;


    bool I_key;

    public bool hasHeadItem;
    void Start()
    {
        Invoke("FindInven", 1f);
    }
    void FindInven()
    {
        InventoryUI = GameObject.FindGameObjectWithTag("InventoryUI");
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
        Debug.Log(ItemList[0]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Head_Item")
        {
            ItemParts obj = other.GetComponent<ItemParts>();

            for(int i = 0; i < 9; ++i)
            {
                if ( ItemList[i] == null)
                {
                    if(obj.type == 3)
                        ItemList[i] = Resources.Load<ItemParts>("Items/Sp_Head_Item");
                    break;
                }
            }
        }
        else if (other.tag == "Body_Item")
        {
            ItemParts obj = other.GetComponent<ItemParts>();

            for (int i = 0; i < 9; ++i)
            {
                if (ItemList[i] == null)
                {
                    if (obj.type == 2)
                        ItemList[i] = Resources.Load<ItemParts>("Items/Sh_Body_Item");
                    break;
                }
            }
        }
        else if (other.tag == "Leg_Item")
        {
            ItemParts obj = other.GetComponent<ItemParts>();

            for (int i = 0; i < 9; ++i)
            {
                if (ItemList[i] == null)
                {
                    if (obj.type == 1)
                        ItemList[i] = Resources.Load<ItemParts>("Items/Po_Leg_Item");
                    break;
                }
            }
        }

    }

}
