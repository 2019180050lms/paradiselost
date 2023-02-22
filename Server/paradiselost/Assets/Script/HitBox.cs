using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{

    public BoxCollider meleeArea;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f); // 0.1√  ¥Î±‚
        meleeArea.enabled = true;
        //trailEffect.enabled = true;

        yield return new WaitForSeconds(0.7f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        //trailEffect.enabled = false;

    }
}
