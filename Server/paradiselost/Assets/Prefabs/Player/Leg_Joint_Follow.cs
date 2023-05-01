using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Joint_Follow : MonoBehaviour
{
    [SerializeField]
    private GameObject neck;
    private Transform neck_transform;
    private Vector3 original_pos;

    private void Awake()
    {
        neck_transform = neck.GetComponent<Transform>();
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
