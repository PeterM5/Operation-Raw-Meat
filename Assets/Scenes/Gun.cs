using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool m_bOnGround = true;
    public GameObject m_bulletPrefab;
    public float m_bulletSpeed = 10;

    public void Shoot(Vector2 fireLocation)
    {
        Vector3 diff = new Vector3(fireLocation.x, fireLocation.y, 0) - transform.position;
        Debug.Log(diff);
        Debug.Log(fireLocation);

        GameObject newBullet = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody2D>().AddForce(diff * m_bulletSpeed);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
