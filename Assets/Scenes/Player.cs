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
                        gun.m_offset = new Vector3(-1.1f, 0);
                        addGun(gun.m_offset, gun);                        
                    }
                    else//right
                    {
                        gun.m_offset = new Vector3(1.1f, 0);
                        addGun(gun.m_offset, gun);                        
                    }
                }
                else
                {
                    if (diff.y > 0)//Below
                    {
                        gun.m_offset = new Vector3(0, -1.1f);
                        addGun(gun.m_offset, gun);                        
                    }
                    else//top
                    {
                        gun.m_offset = new Vector3(0, 1.1f);
                        addGun(gun.m_offset, gun);
                    }
                }                
            }
        }

        //todo check if enemy if so lower health/die
    }

    public void addGun(Vector3 position, Gun newGun)
    {
        if (!m_guns.ContainsValue(position))
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


        //List<GameObject> connectedGuns;

        //Remove disconnected guns
        /*
        List<GameObject> gunsToRemove = new List<GameObject>();
        foreach (KeyValuePair<GameObject, Vector3> gunAndPosition in m_guns)
        {
            if (!isConnectedToPlayer(transform.position + gunAndPosition.Value, 0))
            {
                Debug.Log("Deleting-----------------------------------------------------------------------");
                gunsToRemove.Add(gunAndPosition.Key);
            }          
        }

        foreach(GameObject gunToRemove in gunsToRemove)
        {
            m_guns.Remove(gunToRemove);
            Destroy(gunToRemove);
        }*/
    }

    /*List<GameObject> getConnectedGuns(List<GameObject> connectedGuns, Vector3 position)
    {

        //todo


        if (isGunPosition(position + new Vector3(1.1f, 0, 0)))
        {
            if (isConnectedToPlayer(position + new Vector3(1.1f, 0, 0), depth))
            {
                return true;
            }
        }
    }

    bool isConnectedToPlayer(Vector3 position, int depth)
    {
        depth++;
        if (depth > 10)
        {
            Debug.Log("Depth error");
            return false;
        }
            

        if (isPlayerPosition(position))
        {
            return true;
        }

        if (isPlayerPosition(position + new Vector3(1.1f, 0, 0)))
        {
            return true;
        }
        if (isGunPosition(position + new Vector3(1.1f, 0, 0)))
        {
            if (isConnectedToPlayer(position + new Vector3(1.1f, 0, 0), depth))
            {
                return true;
            }
        }

        if (isPlayerPosition(position + new Vector3(-1.1f, 0, 0)))
        {
            return true;
        }
        if (isGunPosition(position + new Vector3(-1.1f, 0, 0)))
        {
            if (isConnectedToPlayer(position + new Vector3(-1.1f, 0, 0), depth))
            {
                return true;
            }
        }

        if (isPlayerPosition(position + new Vector3(0, 1.1f, 0)))
        {
            return true;
        }
        if (isGunPosition(position + new Vector3(0, 1.1f, 0)))
        {
            if (isConnectedToPlayer(position + new Vector3(0, 1.1f, 0), depth))
            {
                return true;
            }
        }

        if (isPlayerPosition(position + new Vector3(0, -1.1f, 0)))
        {
            return true;
        }
        if (isGunPosition(position + new Vector3(0, -1.1f, 0)))
        {
            if (isConnectedToPlayer(position + new Vector3(0, -1.1f, 0), depth))
            {
                return true;
            }
        }

        return false;
    }

    bool isPlayerPosition(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.name.Contains("Player"))
            {
                return true;
            }
        }
        return false;
    }

    bool isGunPosition(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.name.Contains("gun"))
            {
                return true;
            }
        }
        return false;
    }*/

}
