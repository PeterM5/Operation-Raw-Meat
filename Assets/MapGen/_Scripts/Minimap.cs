using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public Sprite room_11;
    public Sprite room_22;
    public void DrawMap(Dictionary<Vector2Int, Room> mapLayout) {
        // Delete all previously generated map objects
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));

        RectTransform minimap_rt = GetComponent<RectTransform>();
        float minimap_width = minimap_rt.sizeDelta.x;
        float minimap_height = minimap_rt.sizeDelta.y;

        foreach (KeyValuePair<Vector2Int, Room> room in mapLayout) {
            Vector2Int room_pos = room.Key;
            Room room_obj =  room.Value;

            GameObject room_icon_obj = new GameObject("room_icon_" + room_pos.x + "_" + room_pos.y);
            RectTransform rt = room_icon_obj.AddComponent<RectTransform>();
            room_icon_obj.transform.SetParent(transform, false);

            //rt.anchoredPosition = new Vector2(0,0);
            rt.anchorMax = new Vector2(0,0);
            rt.anchorMin = new Vector2(0,0);



            room_icon_obj.AddComponent<CanvasRenderer>();
            Image i = room_icon_obj.AddComponent<Image>();
            if (room_obj.GetRoomSize().x == 1 && room_obj.GetRoomSize().y == 1) {
                i.sprite = room_11;
                rt.sizeDelta = new Vector2(16f, 16f);
                rt.localPosition = new Vector3(room_pos.x * 16f - (minimap_width / 2), room_pos.y * 16f - (minimap_height / 2), 0.0f);
            }
            else if (room_obj.GetRoomSize().x == 2 && room_obj.GetRoomSize().y == 2) {
                i.sprite = room_22;
                rt.sizeDelta = new Vector2(32f, 32f);
                rt.localPosition = new Vector3(room_pos.x * 16f - (minimap_width / 2) + 8, room_pos.y * 16f - (minimap_height / 2) + 8, 0.0f);
            }
        }
    }
}
