using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float magnet_radius = 2f; // Radius of magnet effect with items
    public float magnet_force = 1f; // Force of magnet effect with items
    public float collection_radius = 1.1f; // How close an item has to be to collect it

    public GameObject gun;

    private Rigidbody rb;

    // Stores all Items in the map
    private GameObject[] item_objects;

    // Stores objects stuck to player
    Dictionary<Vector2Int, ItemType> attached_items = new Dictionary<Vector2Int, ItemType>();

    void Start()
    {
        // Retrieve all item game objects in the world
        item_objects = GameObject.FindGameObjectsWithTag("Item");

        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        GetKeyboardInput();
        ProcessItemsInWorld();
    }

    void GetKeyboardInput() {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(new Vector3(0.0f, speed, 0.0f));
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(new Vector3(0.0f, -speed, 0.0f));
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(new Vector3(-speed, 0.0f, 0.0f));
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(new Vector3(speed, 0.0f, 0.0f));
        }
    }

    // This is called every frame
    // We process all item game objects here (to keep everything in one place)
    void ProcessItemsInWorld() {
        foreach (GameObject item_obj in item_objects) {
            Transform item_transform = item_obj.GetComponent<Transform>();
            Rigidbody item_rb = item_obj.GetComponent<Rigidbody>();

            // Move item towards player if close enough
            
            // Calculate distance between Item and Player
            float distance = Vector3.Distance(item_transform.position, transform.position);
            if (distance < magnet_radius) {
                Vector3 force_vector = (transform.position - item_transform.position) * magnet_force / distance;
                item_rb.AddForce(force_vector);
            }

            // Check if item is close enough to player
            
            if (distance < collection_radius) {
                // Join object
                Item item = item_obj.GetComponent<Item>();
                AttachItem(new Vector2Int(-1, 0), item.type);
            }
            

        }
    }

    void AttachItem(Vector2Int pos, ItemType item_type) {
        // Add item to dictionary
        attached_items.Add(pos, item_type);

        // Create GameObject
        switch (item_type) {
            case ItemType.Gun:
                break;
            case ItemType.Shield:
                break;
        }
    }


    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, magnet_radius);
    }

}
