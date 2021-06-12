using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor (typeof (MapGen))]
public class MapGenEditor : Editor 
{
    public override void OnInspectorGUI() {
        MapGen mapGen = (MapGen)target;

        if (DrawDefaultInspector()) { // If any value changed
            if (mapGen.autoUpdate) {
                mapGen.generateMap11();
            }
        }
    }
}
