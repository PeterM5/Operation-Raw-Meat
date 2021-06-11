using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private Vector2Int room_size;
    public Room(int x, int y) {
        this.room_size = new Vector2Int(x, y);
    }
    public Room(Vector2Int room_size) {
        this.room_size = room_size;
    }
    public Vector2Int GetRoomSize() {
        return room_size;
    }
}
