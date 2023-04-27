using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake_Camera : MonoBehaviour
{
    public Transform camtransform;

    public float shakeDuration;

    public float shakeAmount;
    public float decreaseFactor;

    Vector3 originalpos;

    private void Awake()
    {
        shakeDuration = 0f;
        shakeAmount = 0.7f;
        shakeDuration = 100.0f;
        if(camtransform == null)
        {
            camtransform = GetComponent<Transform>();
        }
        

    }

    private void OnEnable()
    {
        originalpos = camtransform.localPosition;
    }

    private void Update()
    {
        if(shakeDuration > 0)
        {
            camtransform.localPosition = originalpos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            camtransform.localPosition = originalpos;
        }
    }
}
