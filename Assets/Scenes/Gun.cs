using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool m_bOnGround = true;
    public GameObject m_bulletPrefab;
    public float m_bulletSpeed = 10;
    public Player m_player;
    public Vector3 m_offset;

    public float m_rotationSpeed = 10;
    public GameObject m_turret;

    public float m_gunGridGap = 0.7f;

    public AudioSource m_audioSource;
    public AudioClip m_shootSound;

    public void Shoot(Vector3 fireLocation)
    {
        Vector3 diff = (fireLocation - transform.position).normalized;

        GameObject newBullet = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().AddForce(diff * m_bulletSpeed);

        m_audioSource.PlayOneShot(m_shootSound, 0.5f);
    }

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_bOnGround)
        {
            Ray castPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, 1000))
            {
                Vector3 direction = hit.point - transform.position;
                direction.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                m_turret.transform.rotation = Quaternion.Slerp(m_turret.transform.rotation, lookRotation, Time.deltaTime * m_rotationSpeed);
            }      
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name.Contains("Enemy"))
        {
            m_player.removeGun(this);
        }
        else if (col.gameObject.name.Contains("Gun"))
        {
            Gun gun = col.gameObject.GetComponent<Gun>();

            //Check its on the ground
            if (gun.m_bOnGround)
            {
                //Determine the position to pick up
                Vector3 diff = transform.position - col.gameObject.transform.position;

                float x = diff.x;
                if (x < 0)
                {
                    x = diff.x * -1;
                }

                float z = diff.z;
                if (z < 0)
                {
                    z = diff.z * -1;
                }

                if (x > z)
                {
                    if (diff.x > 0)//Left
                    {
                        gun.m_offset = m_offset + new Vector3(-m_gunGridGap, 0);
                        m_player.addGun(gun.m_offset, gun);
                    }
                    else//right
                    {
                        gun.m_offset = m_offset + new Vector3(m_gunGridGap, 0);
                        m_player.addGun(gun.m_offset, gun);
                    }
                }
                else
                {
                    if (diff.z > 0)//Below
                    {
                        gun.m_offset = m_offset + new Vector3(0, -m_gunGridGap);
                        m_player.addGun(gun.m_offset, gun);
                    }
                    else//top
                    {
                        gun.m_offset = m_offset + new Vector3(0, m_gunGridGap);
                        m_player.addGun(gun.m_offset, gun);
                    }
                }
            }
        }
    }
}
