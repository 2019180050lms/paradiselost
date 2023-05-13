using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint_Robot : MonoBehaviour
{
    enum parts { head = 0, body = 1, leg = 2};
    public GameObject leg;
    public GameObject body;
    public GameObject head;
    public GameObject[] po_list;
    public GameObject[] sh_list;
    public GameObject[] sp_list;

    public int change_parts;

    public int type = 1;

    public GameObject weaponPoint;
    public GameObject equipWeapon;

    public static Joint_Robot Instance { get; } = new Joint_Robot();

    // Start is called before the first frame update
    void Start()
    {
        po_list = new GameObject[3];
        sh_list = new GameObject[3];
        sp_list = new GameObject[3];


        weaponPoint.transform.position = GameObject.Find("WeaponPoint").transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        //body.transform.position = leg.transform.Find("Joint_Leg").transform.position;
        //head.transform.position = body.transform.Find("Joint_Head").transform.position;
        

        if (Input.GetKeyDown(KeyCode.T)) // 赣府
        {
            change_parts = 1;
          
        }

        if (Input.GetKeyDown(KeyCode.Y)) // 个烹
        {
            change_parts = 2;
        }

        if (Input.GetKeyDown(KeyCode.U)) // 促府
        {
            change_parts = 3;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SwitchParts(1);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SwitchParts(2);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchParts(3);
        }
    }
    
    public void SwitchParts(int type)
    {
        switch (change_parts)
        {
            case 1:  // T 赣府
                switch (type)
                {
                    case 1:
                        //Destroy(head);
                        //head = Instantiate(po_list[(int)parts.head], transform);
                        //head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.position;
                        // quit_canvas = true;
                        equipWeapon = Instantiate(po_list[(int)parts.head], transform);
                        break;
                    case 2:
                        //Destroy(head);
                        //head = Instantiate(sh_list[(int)parts.head], transform);
                        //head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.position;
                        equipWeapon = Instantiate(sh_list[(int)parts.head], transform);
                        // quit_canvas = true;
                        break;
                    case 3:
                        //Destroy(head);
                        //head = Instantiate(sp_list[(int)parts.head], transform);
                        //head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.position;
                        equipWeapon = Instantiate(sp_list[(int)parts.head], transform);
                        //quit_canvas = true;
                        break;
                    default:
                        break;
                }
                break;
            case 2: // Y 个烹
                switch (type)
                {
                    case 1:
                        // quit_canvas = true;
                        break;
                    case 2:
                        //quit_canvas = true;
                        break;
                    case 3:
                        // quit_canvas = true;
                        break;
                    default:
                        break;
                }
                break;
            case 3: // U 促府
                switch (type)
                {
                    case 1:
                        // quit_canvas = true;
                        break;
                    case 2:
                        //quit_canvas = true;
                        break;
                    case 3:
                        //quit_canvas = true;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

}
