using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawn : MonoBehaviour
{
    public GameObject enemy;
    public int xmin_spawn = 8;
    public int xmax_spawn = 8;
    public int ymin_spawn = 8;
    public int ymax_spawn = 8;

    public void SpawnEnemies(int count) {
        for (int i=0; i < 1; i++) {
            int xPos = Random.Range(xmin_spawn, xmax_spawn);
            int yPos = Random.Range(ymin_spawn, ymax_spawn);
            GameObject.Instantiate(enemy, new Vector3(0,2,0), new Quaternion(0,0,0,0));
        }
    }
}
