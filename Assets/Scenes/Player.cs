using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_movementSpeed = 10;
    Dictionary<Vector2, Gun> m_guns;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;

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
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Check if gun
        if(col.gameObject.name.Contains("gun"))
        {
            Gun gun = col.gameObject.GetComponent<Gun>();

            //Check its on the ground
            if(gun.m_bOnGround)
            {
                //Determine the position to pick up
                Vector2 diff = transform.position - col.gameObject.transform.position;
                Debug.Log(diff);
            }
        }

        
    }
}
