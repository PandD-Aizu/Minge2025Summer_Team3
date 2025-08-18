#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoxelMapGenerator))]
public class VoxelMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VoxelMapGenerator mapGen = (VoxelMapGenerator)target;

        if (GUILayout.Button("Generate Map"))
        {
            mapGen.GenerateMap();
        }
    }
}
#endif