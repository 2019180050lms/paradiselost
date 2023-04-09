using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint_Robot : MonoBehaviour
{
    enum parts { head = 0, body = 1, leg = 2};
    GameObject leg;
    GameObject body;
    GameObject head;
    public GameObject[] po_list;
    public GameObject[] sh_list;
    public GameObject[] sp_list;

    //private bool quit_canvas;
    private int change_parts;

    public int type = 1;
    [SerializeField] GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        change_parts = 0;
        leg = Instantiate(sp_list[(int)parts.leg], transform);
        //leg.transform.position = new Vector3(-5, 5, 3);

        body = Instantiate(sp_list[(int)parts.body], transform);
        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition - body.transform.Find("Joint_Leg").transform.localPosition;

        head = Instantiate(sp_list[(int)parts.head], transform);
        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;

        //quit_canvas = false;

        //canvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)) // 赣府
        {
            //StartCoroutine(QuitCanvas());
            change_parts = 1;
          


            //Destroy(head);
            //head = Instantiate(sh_list[(int)parts.head]);
            //head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
        }

        if (Input.GetKeyDown(KeyCode.Y)) // 个烹
        {
            //StartCoroutine(QuitCanvas());
            //Destroy(head);
            //head = Instantiate(sp_list[(int)parts.head]);
            //head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
            change_parts = 2;
        }

        if (Input.GetKeyDown(KeyCode.U)) // 促府
        {
            //StartCoroutine(QuitCanvas());
            //Destroy(head);
            //head = Instantiate(po_list[(int)parts.head]);
            //head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
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

    void SwitchParts(int type)
    {
        switch (change_parts)
        {
            case 1:  // T 赣府
                switch (type)
                {
                    case 1:
                        Destroy(head);
                        head = Instantiate(po_list[(int)parts.head], transform);
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
                        // quit_canvas = true;
                        break;
                    case 2:
                        Destroy(head);
                        head = Instantiate(sh_list[(int)parts.head], transform);
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
                        // quit_canvas = true;
                        break;
                    case 3:
                        Destroy(head);
                        head = Instantiate(sp_list[(int)parts.head], transform);
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
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
                        Destroy(body);
                        body = Instantiate(po_list[(int)parts.body], transform);
                        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition - body.transform.Find("Joint_Leg").transform.localPosition;
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
                        // quit_canvas = true;
                        break;
                    case 2:
                        Destroy(body);
                        body = Instantiate(sh_list[(int)parts.body], transform);
                        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition- body.transform.Find("Joint_Leg").transform.localPosition;
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
                        //quit_canvas = true;
                        break;
                    case 3:
                        Destroy(body);
                        body = Instantiate(sp_list[(int)parts.body], transform);
                        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition - body.transform.Find("Joint_Leg").transform.localPosition;
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
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
                        Destroy(leg);
                        leg = Instantiate(po_list[(int)parts.leg], transform);
                        leg.transform.position = gameObject.transform.position + new Vector3(0, 0.5f, 0);
                        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition - body.transform.Find("Joint_Leg").transform.localPosition;
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
                        // quit_canvas = true;
                        break;
                    case 2:
                        Destroy(leg);
                        leg = Instantiate(sh_list[(int)parts.leg], transform);
                        leg.transform.position = gameObject.transform.position;
                        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition - body.transform.Find("Joint_Leg").transform.localPosition;
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
                        //quit_canvas = true;
                        break;
                    case 3:
                        Destroy(leg);
                        leg = Instantiate(sp_list[(int)parts.leg], transform);
                        leg.transform.position = gameObject.transform.position;
                        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition - body.transform.Find("Joint_Leg").transform.localPosition;
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Body_Item")
        {
            change_parts = 2;
            ItemParts obj = other.GetComponent<ItemParts>();
            int type = obj.type;
            SwitchParts(type);
        }

        else if(other.tag == "Leg_Item")
        {
            change_parts = 3;
            ItemParts obj = other.GetComponent<ItemParts>();
            int type = obj.type;
            SwitchParts(type);
        }

        else if (other.tag == "Head_Item")
        {
            change_parts = 1;
            ItemParts obj = other.GetComponent<ItemParts>();
            int type = obj.type;
            SwitchParts(type);
        }
    }
}
