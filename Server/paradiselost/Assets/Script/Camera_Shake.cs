using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Shake : MonoBehaviour, Shake_transfort
{
    public Transform camtransform;
    public float space_move;
    public float decrease_factor;

    public float shake_duration;

    Vector3 originalpos;
    // Start is called before the first frame update
    private void Awake()
    {
        camtransform = GetComponent<Transform>();
        originalpos = camtransform.localPosition;
        space_move = 0.7f;
        shake_duration = 0f;
        decrease_factor = 0.6f;

        camtransform = GetComponent<Transform>();
    }

    private void Update()
    {
        //originalpos = camtransform.localPosition;
    }

    public void Shake_Camera_Level(int level)
    {
        StartCoroutine(CameraShake_Per_Level(level));
    }

    

    IEnumerator CameraShake_Per_Level(int level)
    {
        switch(level)
        {
            case 1:
                shake_duration = 1.0f;
                space_move = 0.3f;
                while(shake_duration > 0f)
                {
                    camtransform.localPosition = originalpos + Random.insideUnitSphere * space_move;
                    shake_duration -= Time.deltaTime * decrease_factor;
                    yield return new WaitForEndOfFrame();
                    shake_duration -= Time.deltaTime;
                }
                camtransform.localPosition = originalpos;
                break;
            case 2:
                shake_duration = 1.0f;
                space_move = 0.4534f;
                while (shake_duration > 0f)
                {
                    camtransform.localPosition = originalpos + Random.insideUnitSphere * space_move;
                    shake_duration -= Time.deltaTime * decrease_factor;
                    yield return new WaitForEndOfFrame();
                    shake_duration -= Time.deltaTime;
                }
                camtransform.localPosition = originalpos;
                break;
            case 3:
                shake_duration = 0.7f;
                space_move = 0.7f;
                while (shake_duration > 0f)
                {
                    camtransform.localPosition = originalpos + Random.insideUnitSphere * space_move;
                    shake_duration -= Time.deltaTime * decrease_factor;
                    yield return new WaitForEndOfFrame();
                    shake_duration -= Time.deltaTime;
                }
                camtransform.localPosition = originalpos;
                break;
            default:
                break;
        }
    }
}
