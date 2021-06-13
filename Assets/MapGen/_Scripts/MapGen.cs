using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    private Dictionary<Vector2Int, Room> mapLayout; // Key: Position of room. Value: Room
    public GameObject minimap_obj;
    public int seed;
    public int max_no_rooms;
    public bool autoUpdate;
    private Minimap minimap;
    
    public float distance;
    public GameObject[] spawn_rooms;
    public GameObject[] normal_rooms;
    public GameObject[] semimetal_rooms;

    public GameObject enemy;

    public Transform map_parent;
    // Start is called before the first frame update
    void Start()
    {
        generateMap11();
        spawnRoomObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void generateMap11() {
        // Init
        Random.InitState(seed);
        mapLayout = new Dictionary<Vector2Int, Room>();

        // End points in generated map
        List<Vector2Int> not_end_points = new List<Vector2Int>();

        // Queue used in map generation
        Vector2Int[] added_rooms = new Vector2Int[max_no_rooms];
        int tail = -1;
        int length = 0;
        // Add starting room
        tail++;
        added_rooms[0] = new Vector2Int(0,0);
        length++;
        mapLayout.Add(new Vector2Int(0,0), new Room(1,1, RoomType.Spawn));

        bool finish = false;
        long count = 0;
        while(length < max_no_rooms && !finish && count < 100) {
            count ++;
            //foreach (Vector2Int r in added_rooms) { // Loops through queue
            for (int index = tail; index > -1; index--) {
                bool is_end_point = true;
                Vector2Int[] adjacent = randomAdjacent();
                foreach (Vector2Int adj in adjacent) { // Loop through adjacent rooms
                    // 1. Check if max rooms reached
                    if (length >= max_no_rooms) {
                        finish = true;
                        break;
                    }

                    // 2. Check if adjacent room is occupied
                    if (mapLayout.ContainsKey(added_rooms[index] + adj)) continue;
                    
                    // 3. Check if adjacent room has more than one neighbour
                    int num = 0;
                    foreach (Vector2Int a in adjacent) {
                        if (mapLayout.ContainsKey(added_rooms[index] + adj + a)) num++;
                    }
                    if (num > 1) continue;

                    // 4. Random 50% chance to just give up
                    if (Random.Range(0, 1) > 0.5) continue;

                    // All conditions passed! Add adjacent room to queue
                    is_end_point = false;
                    tail++;
                    added_rooms[tail] = added_rooms[index] + adj;
                    length++;
                    mapLayout.Add(added_rooms[index] + adj, new Room(1,1));
                    break;
                }
                if (!is_end_point)
                    not_end_points.Add(added_rooms[index]);
                if (finish || length >= max_no_rooms) break;
            }
        }

        //Obtain end points from non end points
        List<Vector2Int> end_points = new List<Vector2Int>();
        foreach(Vector2Int room in added_rooms) {
            if (!not_end_points.Contains(room)) end_points.Add(room);
        }
        Vector2Int[] end_points_arr = end_points.ToArray();

        // Place special rooms at a random end point

        // Boss room is placed using the last end point, to make it furthest away
        mapLayout[end_points_arr[end_points_arr.GetLength(0)-1]].SetRoomType(RoomType.Boss);

        // Place other special rooms at random end points
        mapLayout[end_points_arr[Random.Range(0, end_points_arr.GetLength(0)-2)]].SetRoomType(RoomType.Coin);
        mapLayout[end_points_arr[Random.Range(0, end_points_arr.GetLength(0)-2)]].SetRoomType(RoomType.Coin);
        mapLayout[end_points_arr[Random.Range(0, end_points_arr.GetLength(0)-2)]].SetRoomType(RoomType.Coin);

        // Draw minimap to UI
        minimap = minimap_obj.GetComponent<Minimap>();
        minimap.DrawMap(mapLayout);
    }

    // Doesnt work fully!!!
    // Check if map is valid (there are no overlapping rooms)
    bool checkMap() {
        foreach (KeyValuePair<Vector2Int, Room> room in mapLayout) {
            Room room_obj = room.Value;
            int room_width = room_obj.GetRoomSize().x;
            int room_height= room_obj.GetRoomSize().y;
            if (room_width != 1 && room_height != 1) {
                Vector2Int room_pos = room.Key;
                // Check each tile
                for (int y=room_pos.y; y<(room_pos.y + room_height); y++) {
                    for (int x=room_pos.x; x<(room_pos.x + room_width); x++) {
                        // Check if any tile is overlapping
                        if (mapLayout.ContainsKey(new Vector2Int(x, y)) && x != room_pos.x && y != room_pos.y) return false;
                    }
                }
            }
        }
        return true;
    }


    // Returns an array of adjacent vectors in a random order
    private Vector2Int[] randomAdjacent() {
        Vector2Int[] adjacent = {new Vector2Int(0,1), new Vector2Int(0,-1), new Vector2Int(1,0), new Vector2Int(-1,0)}; // right left up down
        // Shuffle Array
        for (int i =0; i < adjacent.Length; i++) {
            Vector2Int tmp = adjacent[i];
            int r = Random.Range(i, adjacent.Length);
            adjacent[i] = adjacent[r];
            adjacent[r] = tmp;
        }
        return adjacent;
    }
    
    void spawnRoomObjects() {
        foreach(KeyValuePair<Vector2Int, Room> cell in mapLayout) {
            // Pick room
            GameObject room;
            if (cell.Key.x == 0 && cell.Key.y == 0) {
                room = spawn_rooms[Random.Range(0,spawn_rooms.GetLength(0))];
            }
            else if (normal_rooms.GetLength(0) > 0 && Vector2Int.Distance(cell.Key, new Vector2Int(0,0)) < distance) {
                room = normal_rooms[Random.Range(0,spawn_rooms.GetLength(0))];
            }
            else {
                room = semimetal_rooms[Random.Range(0,spawn_rooms.GetLength(0))];
            }
            GameObject r = GameObject.Instantiate(room, new Vector3(cell.Key.x * 22, 0.0f, cell.Key.y * 22), room.transform.rotation, map_parent);
            int enemy_count = (int)Vector2Int.Distance(cell.Key, new Vector2Int(0,0));
            for (int i=0; i<enemy_count; i++) {
                int xPos = Random.Range(-8, 8);
                int yPos = Random.Range(-8, 8);
                GameObject.Instantiate(enemy, new Vector3(cell.Key.x * 22 + xPos, 2, cell.Key.y * 22 + yPos), new Quaternion(0,0,0,0));
            }
            
            // Check adjacent rooms and remove doors to them
            if (mapLayout.ContainsKey(cell.Key + new Vector2Int(1,0))) { // East
                // Remove east door
                Destroy(r.transform.Find("East_Door").gameObject);
            }
            if (mapLayout.ContainsKey(cell.Key + new Vector2Int(-1,0))) { // West 
                // Remove west door
                Destroy(r.transform.Find("West_Door").gameObject);
            }
            if (mapLayout.ContainsKey(cell.Key + new Vector2Int(0,1))) { // North 
                // Remove north door
                Destroy(r.transform.Find("North_Door").gameObject);
            }
            if (mapLayout.ContainsKey(cell.Key + new Vector2Int(0,-1))) { // South 
                // Remove south door
                Destroy(r.transform.Find("South_Door").gameObject);
            }
        }
    }

}
