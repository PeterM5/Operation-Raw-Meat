using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float m_lifeTime = 5;
    public float m_noIteractLifeTime = 0.5f;
    float m_spawnTime;
    float m_noInteractTime;

    public AudioSource m_audioSource;
    public AudioClip m_ricochetSound;

    // Start is called before the first frame update
    void Start()
    {
        m_spawnTime = Time.time;
        m_noInteractTime = Time.time;
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > m_noInteractTime + m_noIteractLifeTime)
        {
            GetComponent<SphereCollider>().isTrigger = false;
        }
        if (Time.time > m_spawnTime + m_lifeTime)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        m_audioSource.PlayOneShot(m_ricochetSound, 0.5f);
    }
}
