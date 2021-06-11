using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_movementSpeed = 10;
    Dictionary<Vector3, GameObject> m_guns = new Dictionary<Vector3, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;

        foreach (KeyValuePair<Vector3, GameObject> gunAndPosition in m_guns)
        {
            gunAndPosition.Value.transform.position = pos + gunAndPosition.Key;
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
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

            foreach (KeyValuePair<Vector3, GameObject> gunAndPosition in m_guns)
            {
                gunAndPosition.Value.GetComponent<Gun>().Shoot(mousePos);
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

                Debug.Log(diff);

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

        
    }

    void addGun(Vector3 position, Gun newGun)
    {
        if (!m_guns.ContainsKey(position))
        {
            m_guns.Add(position, newGun.gameObject);
            newGun.m_bOnGround = false;

            newGun.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;

            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.offset = position;
        }
        
    }
}
