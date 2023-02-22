using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Camera MainCamera;
    public GameObject Partner;
    public Rigidbody test_rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        test_rigidbody = this.gameObject.GetComponent<Rigidbody>();
        StartCoroutine(LoadingTest());
    }

    // Update is called once per frame
    void Update()
    {
       
        
        if (Partner != null)
        {
            //
           // Partner.transform.localScale += new Vector3(1.0f, 1.0f, 1.0f) * Time.deltaTime;
        }
     
            

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        Partner = collision.gameObject;
        Partner.transform.parent = this.transform;
        //Debug.Log(Partner.gameObject.name);
        Partner.gameObject.AddComponent<Rocet>();
    }

    IEnumerator LoadingTest()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("COroutinecheck");
            if(MainCamera == null)
            {
                break;
            }
        }

        yield return new WaitForEndOfFrame();
        Application.Quit();
    }


}