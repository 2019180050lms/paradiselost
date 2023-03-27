using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    Rigidbody rb;
    public GameObject CameraRotate;
    float power = 30f;

    public float axis_x;
    public float axis_y;


    void Start()
    {
        axis_x = 0;
        axis_y = 0;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        float xmove = Input.GetAxis("Horizontal");
        float ymove = Input.GetAxis("Vertical");

        rb.AddForce(-transform.forward * ymove * 3);
        rb.AddForce(-transform.right * xmove * 3);

        axis_x += Input.GetAxis("Mouse X");
        //axis_y += Input.GetAxis("Mouse Y");
        transform.localEulerAngles = new Vector3(-axis_y, -axis_x, 0);
    }
}
