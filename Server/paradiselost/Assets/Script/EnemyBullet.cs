using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
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
            
            explosionEffect.Emit(100);
            Destroy(gameObject, 1f);
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Floor")
        {
            explosionEffect.Play();
            explosionEffect.Emit(100);
            Debug.Log("fore");
            Destroy(gameObject, 0.2f);
        }
    }

}
