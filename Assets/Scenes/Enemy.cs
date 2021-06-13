using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float min_chase_distance;
    public float speed;
    private GameObject player;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player3d");
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check distance between enemy and player
        float distance = Vector3.Distance(transform.position, player.transform.position);
        
        // If player is close enough, chase!
        if (distance < min_chase_distance) {
            transform.LookAt(player.transform.position);
            transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
            rigidbody.AddForce((player.transform.position - transform.position) * speed / distance, ForceMode.Force);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name.Contains("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}
