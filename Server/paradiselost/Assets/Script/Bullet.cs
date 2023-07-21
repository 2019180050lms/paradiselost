using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public int ParentID;
    public ParticleSystem explosionEffect;
    private void Start()
    {
        explosionEffect = GetComponentInChildren<ParticleSystem>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
            explosionEffect.Emit(100);
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Floor")
        {
            explosionEffect.Play();
            explosionEffect.Emit(100);
            Debug.Log("fore");
            Destroy(gameObject, 0.5f);
        }
    }

}
