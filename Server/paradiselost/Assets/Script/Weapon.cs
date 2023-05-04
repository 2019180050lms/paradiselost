using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;


    public Transform bulletPos;
    public GameObject bullet;

    public int maxAmmo;
    public int curAmmo;
    public int ParentId;

    void Start()
    {
        Player ParentPlayer = GetComponentInParent<Player>();
        ParentId = ParentPlayer.PlayerId;
    }
    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f); // 0.1√  ¥Î±‚
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.2f);
        trailEffect.enabled = false;

    }


}
