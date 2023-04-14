using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint_Robot : MonoBehaviour
{
    enum parts { head = 0, body = 1, leg = 2};
    public GameObject leg;
    public GameObject body;
    public GameObject  head;
    public GameObject[] po_list;
    public GameObject[] sh_list;
    public GameObject[] sp_list;

    //private bool quit_canvas;
    public int change_parts;

    public int type = 1;
    [SerializeField] GameObject canvas;

    public static Joint_Robot Instance { get; } = new Joint_Robot();

    // Start is called before the first frame update
    void Start()
    {
        //quit_canvas = false;

        //canvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)) // 赣府
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

}
