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
    // Start is called before the first frame update
    void Start()
    {
        leg =  Instantiate(po_list[(int)parts.leg]);
        leg.transform.position = new Vector3(0, 0, 0);

        body = Instantiate(po_list[(int)parts.body]);
        body.transform.position = leg.transform.position + leg.transform.Find("Joint_Leg").transform.localPosition - body.transform.Find("Joint_Leg").transform.localPosition;

        head =  Instantiate(po_list[(int)parts.head]);
        head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;


    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Destroy(head);
            head = Instantiate(sh_list[(int)parts.head]);
            head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Destroy(head);
            head = Instantiate(sp_list[(int)parts.head]);
            head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Destroy(head);
            head = Instantiate(po_list[(int)parts.head]);
            head.transform.position = body.transform.position + body.transform.Find("Joint_Head").transform.localPosition - head.transform.Find("Joint_Head").transform.localPosition;
        }
    }
}
