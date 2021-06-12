using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_movementSpeed = 10;

    Dictionary<GameObject, Vector3> m_guns = new Dictionary<GameObject, Vector3>();
    Dictionary<GameObject, BoxCollider2D> m_addtionalColliders = new Dictionary<GameObject, BoxCollider2D>();

    float m_timeOfLastShot;
    public float m_shotInterval = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        m_timeOfLastShot = Time.time;
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;

        foreach (KeyValuePair<GameObject, Vector3> gunAndPosition in m_guns)
        {
            gunAndPosition.Key.transform.position = pos + gunAndPosition.Value;
        }

        if (Input.GetKey("w"))
        {
            pos.y += m_movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.y -= m_movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += m_movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= m_movementSpeed * Time.deltaTime;
        }

        transform.position = pos;

        if (Input.GetMouseButton(0))
        {
            if (Time.time > m_timeOfLastShot + m_shotInterval)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

                foreach (KeyValuePair<GameObject, Vector3> gunAndPosition in m_guns)
                {
                    gunAndPosition.Key.GetComponent<Gun>().Shoot(mousePos);
                }

                m_timeOfLastShot = Time.time;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Check if gun
        if(col.gameObject.name.Contains("gun"))
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

                if (x > y)
                {
                    if(diff.x > 0)//Left
                    {
                        addGun(new Vector3(-1.1f, 0), gun);                        
                    }
                    else//right
                    {
                        addGun(new Vector3(1.1f, 0), gun);                        
                    }
                }
                else
                {
                    if (diff.y > 0)//Below
                    {
                        addGun(new Vector3(0, -1.1f), gun);                        
                    }
                    else//top
                    {
                        addGun(new Vector3(0, 1.1f), gun);
                    }
                }                
            }
        }

        //todo check if enemy if so lower health/die
    }

    public void addGun(Vector3 position, Gun newGun)
    {
        if (!m_guns.ContainsKey(newGun.gameObject))
        {
            m_guns.Add(newGun.gameObject, position);
            newGun.m_bOnGround = false;
            newGun.m_player = gameObject.GetComponent<Player>();

            newGun.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;

            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.offset = position;

            m_addtionalColliders.Add(newGun.gameObject, boxCollider);
        }
        
    }

    public void removeGun(Gun gun)
    {
        //destroy gun
        m_guns.Remove(gun.gameObject);

        //Destroy extra box collider
        BoxCollider2D boxCollider;
        if (m_addtionalColliders.TryGetValue(gun.gameObject, out boxCollider))
        {
            Destroy(boxCollider);
            m_addtionalColliders.Remove(gun.gameObject);
        }

        Destroy(gun.gameObject);
    }
}
