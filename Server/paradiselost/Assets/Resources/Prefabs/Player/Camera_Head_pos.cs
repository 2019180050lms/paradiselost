using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Head_pos : MonoBehaviour
{

    public Transform head_transform;

    //카메라 저장
    public GameObject eye_View;
    public GameObject sholder_View;

    public bool is_eye;
    public bool is_sholder;

    
    // Start is called before the first frame update
    void Start()
    {
        is_eye = false;
        is_sholder = true;
        head_transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
       
            is_eye = !is_eye;
            is_sholder = !is_sholder;
            sholder_View.GetComponent<Camera>().enabled = is_sholder;
            eye_View.GetComponent<Camera>().enabled = is_eye;
      


    }
}
