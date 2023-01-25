using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocet : MonoBehaviour
{
    float current_time = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        current_time += Time.deltaTime;
        this.gameObject.transform.position = new Vector3(100 * Mathf.Sin(current_time * Mathf.Deg2Rad), 100 * Mathf.Cos(current_time * Mathf.Deg2Rad), 0);
    }
}
