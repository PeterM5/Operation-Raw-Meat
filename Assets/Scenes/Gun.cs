using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool m_bOnGround = true;
    public GameObject m_bulletPrefab;
    public float m_bulletSpeed = 10;
    public Player m_player;

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

    void OnTriggerEnter2D(Collider2D col)
    {
                    /*
        if (col.gameObject.name.Contains("Enemy"))
        {
            m_player.removeGun(this);
        }
        else if (col.gameObject.name.Contains("gun"))
        {

            Gun gun = col.gameObject.GetComponent<Gun>();

            //Check its on the ground
            if (gun.m_bOnGround)
            {
                //Determine the position to pick up
                Vector2 diff = transform.position - col.gameObject.transform.position;

                float x = diff.x;
                if (x < 0)
                {
                    x = diff.x * -1;
                }

                float y = diff.y;
                if (y < 0)
                {
                    y = diff.y * -1;
                }

                Debug.Log(m_player.transform.position);
                Debug.Log("teehee");

                if (x > y)
                {
                    if (diff.x > 0)//Left
                    {
                        m_player.addGun(m_player.transform.position - transform.position + new Vector3(-1.1f, 0), gun);
                    }
                    else//right
                    {
                        m_player.addGun(m_player.transform.position - transform.position + new Vector3(1.1f, 0), gun);
                    }
                }
                else
                {
                    if (diff.y > 0)//Below
                    {
                        m_player.addGun(m_player.transform.position - transform.position + new Vector3(0, -1.1f), gun);
                    }
                    else//top
                    {
                        m_player.addGun(m_player.transform.position - transform.position + new Vector3(0, 1.1f), gun);
                    }
                }
            }
        }*/
    }
}
