using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Cam_Controller : MonoBehaviour
{
    public GameObject camera_object;
    Camera_Shake camerashake;
    // Start is called before the first frame update
    void Start()
    {
        camerashake = camera_object.GetComponent<Camera_Shake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            camerashake.Shake_Camera_Level(1);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            camerashake.Shake_Camera_Level(2);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            camerashake.Shake_Camera_Level(3);
        }
    }
}
