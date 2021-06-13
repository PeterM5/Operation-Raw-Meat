using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //Time spent out of suit progress bar
    public Slider m_outOfSuitProgressBar;
    public float m_maxOutOfSuitTime = 30;
    private float m_outOfSuitTimeLeft;

    public GameObject m_playerBody;
    private Rigidbody m_playerBodyRigidBody;

    public GameObject m_gameOverPanel;

    private bool m_bGameOver = false;

    public float m_gunGridGap = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        m_timeOfLastShot = Time.time;
        m_timeOfLastSuitChange = Time.time;

        m_rigidBody = GetComponent<Rigidbody>();
        m_playerBodyRigidBody = m_playerBody.GetComponent<Rigidbody>();

        m_outOfSuitProgressBar.gameObject.SetActive(false);
        m_outOfSuitTimeLeft = m_maxOutOfSuitTime;

        m_gameOverPanel.SetActive(false);
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
        if(!m_bGameOver)
        {
            if (!m_bOutOfSuit)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    m_rigidBody.AddForce(new Vector3(0.0f, 0.0f, m_movementSpeed));
                }
                if (Input.GetKey(KeyCode.S))
                {
                    m_rigidBody.AddForce(new Vector3(0.0f, 0.0f, -m_movementSpeed));
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
                    m_playerBodyRigidBody.AddForce(new Vector3(0.0f, 0.0f, m_movementSpeed));
                }
                if (Input.GetKey(KeyCode.S))
                {
                    m_playerBodyRigidBody.AddForce(new Vector3(0.0f, 0.0f, -m_movementSpeed));
                }
                if (Input.GetKey(KeyCode.A))
                {
                    m_playerBodyRigidBody.AddForce(new Vector3(-m_movementSpeed, 0.0f, 0.0f));
                }
                if (Input.GetKey(KeyCode.D))
                {
                    m_playerBodyRigidBody.AddForce(new Vector3(m_movementSpeed, 0.0f, 0.0f));
                }

                //Calculate remaingn time out of suit
                m_outOfSuitTimeLeft -= Time.deltaTime;
                float fractionTimeLeft = (m_outOfSuitTimeLeft / m_maxOutOfSuitTime);
                m_outOfSuitProgressBar.value = fractionTimeLeft;

                if (m_outOfSuitTimeLeft < 0)
                {
                    GameOver();
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
                        m_outOfSuitTimeLeft = m_maxOutOfSuitTime;
                        m_outOfSuitProgressBar.value = 1;
                        m_outOfSuitProgressBar.gameObject.SetActive(false);
                        m_playerBody.GetComponent<BoxCollider>().isTrigger = true;
                    }
                }
                else
                {
                    m_bOutOfSuit = true;
                    m_outOfSuitProgressBar.gameObject.SetActive(true);

                    m_playerBody.GetComponent<BoxCollider>().isTrigger = false;
                }
                m_timeOfLastSuitChange = Time.time;
            }

            if(Input.GetKey(KeyCode.R))
            {
                foreach (KeyValuePair<GameObject, Vector3> gunAndPosition in m_guns)
                {
                    gunAndPosition.Key.GetComponent<Gun>().m_bOnGround = true;
                }
                m_guns.Clear();
            }

            if (Input.GetMouseButton(0))
            {
                if (Time.time > m_timeOfLastShot + m_shotInterval)
                {
                    Ray castPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(castPoint, out hit, 1000))
                    {
                        foreach (KeyValuePair<GameObject, Vector3> gunAndPosition in m_guns)
                        {
                            gunAndPosition.Key.GetComponent<Gun>().Shoot(hit.point);
                        }
                    }

                    m_timeOfLastShot = Time.time;
                }
            }
        }
    }

    private void GameOver()
    {
        m_bGameOver = true;
        m_gameOverPanel.SetActive(true);
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
                        gun.m_offset = new Vector3(-m_gunGridGap, 0);
                        addGun(gun.m_offset, gun);
                    }
                    else//right
                    {
                        gun.m_offset = new Vector3(m_gunGridGap, 0);
                        addGun(gun.m_offset, gun);
                    }
                }
                else
                {
                    if (diff.z > 0)//Below
                    {
                        gun.m_offset = new Vector3(0, 0, -m_gunGridGap);
                        addGun(gun.m_offset, gun);
                    }
                    else//top
                    {
                        gun.m_offset = new Vector3(0, 0, m_gunGridGap);
                        addGun(gun.m_offset, gun);
                    }
                }
            }
        }

        //todo check if enemy if so lower health/die
        //Check if gun
        if (col.gameObject.name.Contains("Enemy"))
        {
            GameOver();
        }
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

        Collider[] colliders = Physics.OverlapSphere(position + new Vector3(m_gunGridGap, 0), 0.5f);
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
                    connectedGuns = getConnectedGuns(connectedGuns, position + new Vector3(m_gunGridGap, 0), deletedGameObject, depth);
                }
                
            }
        }

        colliders = Physics.OverlapSphere(position + new Vector3(-m_gunGridGap, 0), 0.5f);
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
                    connectedGuns = getConnectedGuns(connectedGuns, position + new Vector3(-m_gunGridGap, 0), deletedGameObject, depth);
                }                
            }
        }

        colliders = Physics.OverlapSphere(position + new Vector3(0, 0, m_gunGridGap), 0.5f);
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
                    connectedGuns = getConnectedGuns(connectedGuns, position + new Vector3(0, 0, m_gunGridGap), deletedGameObject, depth);
                }                
            }
        }

        colliders = Physics.OverlapSphere(position + new Vector3(0, 0, -m_gunGridGap), 0.5f);
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
                    connectedGuns = getConnectedGuns(connectedGuns, position + new Vector3(0, 0, -m_gunGridGap), deletedGameObject, depth);
                }               
            }
        }

        return connectedGuns;
    }
}
