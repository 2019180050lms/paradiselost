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

    private bool quit_canvas;
    private int change_parts;


    [SerializeField] GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        change_parts = 0;
        leg =  Instantiate(po_list[(int)parts.leg], transform);
        leg.transform.position = new Vector3(0, 0, 0);

        body = Instantiate(po_list[(int)parts.body], transform);
        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition - body.transform.Find("Joint_Leg").transform.localPosition;

        head =  Instantiate(po_list[(int)parts.head], transform);
        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;

        quit_canvas = false;

        canvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(QuitCanvas());
            change_parts = 1;
          


            //Destroy(head);
            //head = Instantiate(sh_list[(int)parts.head]);
            //head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(QuitCanvas());
            //Destroy(head);
            //head = Instantiate(sp_list[(int)parts.head]);
            //head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
            change_parts = 2;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(QuitCanvas());
            //Destroy(head);
            //head = Instantiate(po_list[(int)parts.head]);
            //head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
            change_parts = 3;
        }

        int type = canvas.GetComponent<Canvas_Change_Robot>().checkNum;

        switch(change_parts)
        {
            case 1:
                switch (type)
                {
                    case 1:
                        Destroy(head);
                        head = Instantiate(po_list[(int)parts.head], transform);
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
                        quit_canvas = true;
                        break;
                    case 2:
                        Destroy(head);
                        head = Instantiate(sh_list[(int)parts.head], transform);
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
                        quit_canvas = true;
                        break;
                    case 3:
                        Destroy(head);
                        head = Instantiate(sp_list[(int)parts.head], transform);
                        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
                        quit_canvas = true;
                        break;
                    default:
                        break;
                }
                break;
            case 2:
                switch (type)
                {
                    case 1:
                        Destroy(body);
                        body = Instantiate(po_list[(int)parts.body], transform);
                        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition - body.transform.Find("Joint_Leg").transform.localPosition;
                        quit_canvas = true;
                        break;
                    case 2:
                        Destroy(body);
                        body = Instantiate(sh_list[(int)parts.body], transform);
                        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition - body.transform.Find("Joint_Leg").transform.localPosition;
                        quit_canvas = true;
                        break;
                    case 3:
                        Destroy(body);
                        body = Instantiate(sp_list[(int)parts.body], transform);
                        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition - body.transform.Find("Joint_Leg").transform.localPosition;
                        quit_canvas = true;
                        break;
                    default:
                        break;
                }
                break;
            case 3:
                switch (type)
                {
                    case 1:
                        Destroy(leg);
                        leg = Instantiate(po_list[(int)parts.leg], transform);
                        leg.transform.position = gameObject.transform.position;
                        quit_canvas = true;
                        break;
                    case 2:
                        Destroy(leg);
                        leg = Instantiate(sh_list[(int)parts.leg], transform);
                        leg.transform.position = gameObject.transform.position;
                        quit_canvas = true;
                        break;
                    case 3:
                        Destroy(leg);
                        leg = Instantiate(sp_list[(int)parts.leg], transform);
                        leg.transform.position = gameObject.transform.position;
                        quit_canvas = true;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
       
    }

    IEnumerator QuitCanvas()
    {
        canvas.gameObject.SetActive(true);
        yield return new WaitUntil(() => quit_canvas);
        quit_canvas = false;
        canvas.gameObject.SetActive(false);
    }
}
