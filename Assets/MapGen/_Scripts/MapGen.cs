using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    private Dictionary<Vector2Int, Room> mapLayout; // Key: Position of room. Value: Room
    public GameObject[] rooms;
    public GameObject minimap_obj;
    private Minimap minimap;
    // Start is called before the first frame update
    void Start()
    {
        mapLayout = new Dictionary<Vector2Int, Room>();
        mapLayout.Add(new Vector2Int(0,0), new Room(1,1));
        mapLayout.Add(new Vector2Int(1,0), new Room(1,1));
        mapLayout.Add(new Vector2Int(1,1), new Room(2,2));

        minimap = minimap_obj.GetComponent<Minimap>();
        minimap.DrawMap(mapLayout);
        Debug.Log(checkMap());
    }

    // Update is called once per frame
    void Update()
    {
        
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
