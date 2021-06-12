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

    //Getting in and out of suit
    private bool m_bOutOfSuit = false;
    private float m_timeOfLastSuitChange;
    public float m_suitChangeInterval = 0.1f;
    public float m_distanceToGetBackInSuit = 2f;

    public GameObject m_playerBody;
    private Rigidbody m_playerBodyRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        m_timeOfLastShot = Time.time;
        m_timeOfLastSuitChange = Time.time;
        m_rigidBody = GetComponent<Rigidbody>();
        m_playerBodyRigidBody = m_playerBody.GetComponent<Rigidbody>();
    }

    void Update()
    {
        foreach (KeyValuePair<GameObject, Vector3> gunAndPosition in m_guns)
        {
            gunAndPosition.Key.transform.position = transform.position + gunAndPosition.Value;
        }

        if(!m_bOutOfSuit)
        {
            m_playerBody.transform.position = transform.position;
        }
    }

    void FixedUpdate()
    {
        if(!m_bOutOfSuit)
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
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                m_playerBodyRigidBody.AddForce(new Vector3(0.0f, m_movementSpeed, 0.0f));
            }
            if (Input.GetKey(KeyCode.S))
            {
                m_playerBodyRigidBody.AddForce(new Vector3(0.0f, -m_movementSpeed, 0.0f));
            }
            if (Input.GetKey(KeyCode.A))
            {
                m_playerBodyRigidBody.AddForce(new Vector3(-m_movementSpeed, 0.0f, 0.0f));
            }
            if (Input.GetKey(KeyCode.D))
            {
                m_playerBodyRigidBody.AddForce(new Vector3(m_movementSpeed, 0.0f, 0.0f));
            }
        }

        //Toggle getting in/out of suit
        if (Input.GetKey(KeyCode.E) && Time.time > m_timeOfLastSuitChange + m_suitChangeInterval)
        {
            if (m_bOutOfSuit)
            {
                //Check were close enough to get back in the suit
                Vector3 distanceBetweenPlayerAndSuit = m_playerBody.transform.position - transform.position;
                if (distanceBetweenPlayerAndSuit.x < m_distanceToGetBackInSuit && distanceBetweenPlayerAndSuit.x > -m_distanceToGetBackInSuit && distanceBetweenPlayerAndSuit.y < m_distanceToGetBackInSuit && distanceBetweenPlayerAndSuit.y > -m_distanceToGetBackInSuit)
                {
                    m_bOutOfSuit = false;
                }
            }
            else
            {
                m_bOutOfSuit = true;
            }            
            m_timeOfLastSuitChange = Time.time;
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
        if (!m_guns.ContainsValue(position) && !m_guns.ContainsKey(newGun.gameObject))
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
        //Get list of connected guns
        //Check agaisnt entire list of guns
        //Disconnect non connected guns

        List<GameObject> connectedGuns = getConnectedGuns(new List<GameObject>(), transform.position, gun.gameObject, 0);
        List<GameObject> gunsToRemove = new List<GameObject>();
        foreach (KeyValuePair<GameObject, Vector3> gunAndPosition in m_guns)
        {
            if(!connectedGuns.Contains(gunAndPosition.Key))
            {                
                gunsToRemove.Add(gunAndPosition.Key);
            }      
        }

        foreach(GameObject gunToRemove in gunsToRemove)
        {
            m_guns.Remove(gunToRemove);
            Destroy(gunToRemove);
            //gunToRemove.GetComponent<Gun>().m_bOnGround = true;            
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
}
