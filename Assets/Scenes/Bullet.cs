using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float m_lifeTime = 5;
    float m_spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        m_spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > m_spawnTime + m_lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
