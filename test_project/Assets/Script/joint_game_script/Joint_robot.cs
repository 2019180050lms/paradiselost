using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint_robot : MonoBehaviour
{
    public GameObject Leg_object;
    public GameObject Body_object;
    public GameObject Head_object;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 leg_pivot = Leg_object.transform.Find("pivot_body").position;
        Body_object.transform.position = leg_pivot;
        Vector3 head_pivot = Body_object.transform.Find("pivot_head").position;
        Head_object.transform.position = head_pivot - Head_object.transform.Find("pivot_body").localPosition;
    }
}
