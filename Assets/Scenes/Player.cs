using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_movementSpeed = 10;

    Dictionary<GameObject, Vector3> m_guns = new Dictionary<GameObject, Vector3>();
    Dictionary<GameObject, BoxCollider> m_addtionalColliders = new Dictionary<GameObject, BoxCollider>();

    float m_timeOfLastShot;
    public float m_shotInterval = 0.05f;

    private Rigidbody m_rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        m_timeOfLastShot = Time.time;
        m_rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        foreach (KeyValuePair<GameObject, Vector3> gunAndPosition in m_guns)
        {
            gunAndPosition.Key.transform.position = transform.position + gunAndPosition.Value;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            m_rigidBody.AddForce(new Vector3(0.0f, m_movementSpeed, 0.0f));
        }
        if (Input.GetKey(KeyCode.S))
        {
            m_rigidBody.AddForce(new Vector3(0.0f, -m_movementSpeed, 0.0f));
        }
        if (Input.GetKey(KeyCode.A))
        {
            m_rigidBody.AddForce(new Vector3(-m_movementSpeed, 0.0f, 0.0f));
        }
        if (Input.GetKey(KeyCode.D))
        {
            m_rigidBody.AddForce(new Vector3(m_movementSpeed, 0.0f, 0.0f));
        }

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

    private void OnTriggerEnter(Collider col)
    {
        //Check if gun
        if (col.gameObject.name.Contains("Gun"))
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
                    if (diff.x > 0)//Left
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

            newGun.gameObject.GetComponent<BoxCollider>().isTrigger = true;

            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = position;

            m_addtionalColliders.Add(newGun.gameObject, boxCollider);
        }
        
    }

    public void removeGun(Gun gun)
    {

        //destroy gun
        m_guns.Remove(gun.gameObject);

        //Destroy extra box collider
        BoxCollider boxCollider;
        if (m_addtionalColliders.TryGetValue(gun.gameObject, out boxCollider))
        {
            Destroy(boxCollider);
            m_addtionalColliders.Remove(gun.gameObject);
        }

        Destroy(gun.gameObject);


        //Remove disconnected guns


        List<GameObject> connectedGuns = getConnectedGuns(new List<GameObject>(), transform.position, gun.gameObject, 0);
        Debug.Log(connectedGuns.Count);

        List<GameObject> gunsToRemove = new List<GameObject>();
        foreach (KeyValuePair<GameObject, Vector3> gunAndPosition in m_guns)
        {
            if(!connectedGuns.Contains(gunAndPosition.Key))
            {
                Debug.Log("Deleting-----------------------------------------------------------------------");
                gunsToRemove.Add(gunAndPosition.Key);
            }      
        }

        foreach(GameObject gunToRemove in gunsToRemove)
        {
            m_guns.Remove(gunToRemove);
            Destroy(gunToRemove);
        }
    }

    List<GameObject> getConnectedGuns(List<GameObject> connectedGuns, Vector3 position, GameObject deletedGameObject, int depth)
    {
        depth++;

        if(depth > 10)
        {
            return connectedGuns;
        }

        Collider[] colliders = Physics.OverlapSphere(position + new Vector3(1.1f, 0), 0.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.name.Contains("Gun"))
            {
                if (collider.gameObject != deletedGameObject)
                {
                    if (!connectedGuns.Contains(collider.gameObject))
                    {
                        connectedGuns.Add(collider.gameObject);
                    }
                    connectedGuns = getConnectedGuns(connectedGuns, position + new Vector3(1.1f, 0), deletedGameObject, depth);
                }
                
            }
        }

        colliders = Physics.OverlapSphere(position + new Vector3(-1.1f, 0), 0.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.name.Contains("Gun"))
            {
                if(collider.gameObject != deletedGameObject)
                {
                    if (!connectedGuns.Contains(collider.gameObject))
                    {
                        connectedGuns.Add(collider.gameObject);
                    }
                    connectedGuns = getConnectedGuns(connectedGuns, position + new Vector3(-1.1f, 0), deletedGameObject, depth);
                }                
            }
        }

        colliders = Physics.OverlapSphere(position + new Vector3(0, 1.1f), 0.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.name.Contains("Gun"))
            {
                if (collider.gameObject != deletedGameObject)
                {
                    if (!connectedGuns.Contains(collider.gameObject))
                    {
                        connectedGuns.Add(collider.gameObject);
                    }
                    connectedGuns = getConnectedGuns(connectedGuns, position + new Vector3(0, 1.1f), deletedGameObject, depth);
                }                
            }
        }

        colliders = Physics.OverlapSphere(position + new Vector3(0, -1.1f), 0.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.name.Contains("Gun"))
            {
                if (collider.gameObject != deletedGameObject)
                {
                    if (!connectedGuns.Contains(collider.gameObject))
                    {
                        connectedGuns.Add(collider.gameObject);
                    }
                    connectedGuns = getConnectedGuns(connectedGuns, position + new Vector3(0, -1.1f), deletedGameObject, depth);
                }               
            }
        }

        return connectedGuns;
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
    }

}
