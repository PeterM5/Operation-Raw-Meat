using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Normal, Spawn, Coin, Boss}

public class Room
{
    private Vector2Int room_size;
    private RoomType room_type;
    public Room(int x, int y, RoomType type = RoomType.Normal) {
        this.room_size = new Vector2Int(x, y);
        this.room_type = type;
    }
    public Room(Vector2Int room_size, RoomType type = RoomType.Normal) {
        this.room_size = room_size;
        this.room_type = type;
    }
    public Vector2Int GetRoomSize() {
        return room_size;
    }

    public RoomType GetRoomType() {
        return room_type;
    }

    public void SetRoomType(RoomType type) {
        this.room_type = type;
    }

}
