using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClear : MonoBehaviour
{
    // Start is called before the first frame update\
    public SoundManager soundManager;
    public AudioSource audioSourceBgm;

    void Start()
    {
        soundManager = GetComponent<SoundManager>();
        audioSourceBgm = gameObject.AddComponent<AudioSource>();
        audioSourceBgm.clip = soundManager.Bgm;
        audioSourceBgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
