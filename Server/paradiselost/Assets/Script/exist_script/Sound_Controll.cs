using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Controll : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject check;
    public AudioClip[] clips;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = check.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            audioSource.clip = clips[0];
            audioSource.Play();
           
        }
    }
}
