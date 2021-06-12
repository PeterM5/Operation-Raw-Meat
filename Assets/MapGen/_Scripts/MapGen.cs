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
    // Start is called before the first frame update
    void Start()
    {
        generateMap11();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void generateMap11() {
        Random.InitState(seed);
        mapLayout = new Dictionary<Vector2Int, Room>();

        //Queue<Vector2Int> added_rooms = new Queue<Vector2Int>();
        Vector2Int[] added_rooms = new Vector2Int[max_no_rooms];
        int tail = -1;
        int length = 0;
        // Add starting room
        tail++;
        added_rooms[0] = new Vector2Int(0,0);
        length++;
        mapLayout.Add(new Vector2Int(0,0), new Room(1,1));

        bool finish = false;
        long count = 0;
        while(length < max_no_rooms && !finish && count < 100) {
            count ++;
            //foreach (Vector2Int r in added_rooms) { // Loops through queue
            for (int index = tail; index > -1; index--) {
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
                    tail++;
                    added_rooms[tail] = added_rooms[index] + adj;
                    length++;
                    mapLayout.Add(added_rooms[index] + adj, new Room(1,1));
                    break;
                }
                if (finish || length >= max_no_rooms) break;
            }
        }
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
}
