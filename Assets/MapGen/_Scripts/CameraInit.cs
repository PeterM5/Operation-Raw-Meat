using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInit : MonoBehaviour
{
    public float speed = 1f; // Speed of rotation and zoom
    public float zoom = 5f; // How much to zoom out
    public float interpolate = 0.5f; // 0 means next movement starts right after the previous, 0.5 means next movement starts during the end of previous. PI means all movement at the same time. > PI is equivilant to PI

    public float start_after; // Wait seconds before initalizing camera
    private float wait;

    public GameObject cameraHolder; // Parent gameobject that we rotate
    private Camera this_camera;

    private float lerp1;
    private float lerp2;
    private float lerp3;


    // Start is called before the first frame update
    void Start()
    {
        this_camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (wait > start_after) {
            if (interpolate < 0) { // Smaller than 0 should give a bounce effect with the camera
                if (lerp1 < Mathf.PI - interpolate)
                    lerp1 += speed * Time.deltaTime;
                else if (lerp2 < Mathf.PI - interpolate)
                    lerp2 += speed * Time.deltaTime;
                else if (lerp3 < Mathf.PI - interpolate)
                    lerp3 += speed * Time.deltaTime;
            } else { // Greater than 0 should smooth all camera movements together
                if (lerp1 < Mathf.PI)
                    lerp1 += speed * Time.deltaTime;
                if (lerp2 < Mathf.PI && lerp1 >= Mathf.PI - interpolate)
                    lerp2 += speed * Time.deltaTime;
                if (lerp3 < Mathf.PI && lerp1 >= Mathf.PI - interpolate && lerp2 >= Mathf.PI - interpolate)
                    lerp3 += speed * Time.deltaTime;
            }

            cameraHolder.transform.eulerAngles = new Vector3((1-Mathf.Cos(lerp1)) * 45 / 2, (1-Mathf.Cos(lerp2)) * 45 / 2, 0.0f);
            this_camera.orthographicSize = 5 + (1-Mathf.Cos(lerp3)) * zoom / 2;

        } else wait += 1 * Time.deltaTime;
    }
}