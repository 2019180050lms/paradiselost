using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_Joint_follow : MonoBehaviour
{
    [SerializeField]
    private GameObject neck;

    public GameObject center;
    private Transform center_pos;
    
    private Transform neck_transform;
    private Vector3 original_pos;

    private void Awake()
    {


        neck_transform = neck.GetComponent<Transform>();
        center_pos = center.GetComponent<Transform>();
        
        transform.position = neck_transform.position;
        original_pos = transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        original_pos = neck_transform.position;


        transform.position = original_pos;
    }
}
