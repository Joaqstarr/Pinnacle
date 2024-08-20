using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Random = UnityEngine.Random;


public class PlacementEditorWindow : EditorWindow
{
    private Texture2D _noiseMapTexture;
    private float _density = 0.5f;
    private GameObject _prefab;
    
    [MenuItem("Tools/Joaq/Spawn Items on Terrain")]
    private static void ShowWindow()
    {
        var window = GetWindow<PlacementEditorWindow>("Prefab Placement");
       // window.titleContent = new GUIContent("TITLE");
       // window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        _noiseMapTexture =
            (Texture2D)EditorGUILayout.ObjectField("Noise Map Texture", _noiseMapTexture, typeof(Texture2D), false);
        if (GUILayout.Button("Generate Noise"))
        {
            int width = (int)Terrain.activeTerrain.terrainData.size.x;
            int height = (int)Terrain.activeTerrain.terrainData.size.y;

            float scale = 5;
            
            //TODO NOISE
            _noiseMapTexture = Noise.GetNoiseMap(width, height, scale);
        }
        EditorGUILayout.EndHorizontal();
        _density = EditorGUILayout.Slider("Density", _density, 0, 1);
        
        _prefab = (GameObject)EditorGUILayout.ObjectField("Object Prefab", _prefab, typeof(GameObject), false);

        if (GUILayout.Button("Place Objects"))
        {
            PlaceObjects(Terrain.activeTerrain, _noiseMapTexture, _density, _prefab ,new Vector2Int(10, 10));
        }
        
    }

    public static void PlaceObjects(Terrain terrain, Texture2D noiseMapTexture, float density, GameObject prefab, Vector2Int chunkAmount)
    {
        Transform parent = new GameObject("PlacedObjects").transform;

        //List<Vector3>[] groups = new List<Vector3>[chunkAmount.x * chunkAmount.y];
        

        for (int x = 0; x < terrain.terrainData.size.x; x++)
        {
            for (int z = 0; z < terrain.terrainData.size.z; z++)
            {
                

                if (Fitness(terrain, noiseMapTexture, x, z) > 1 - density)
                {
                    float xOffset = UnityEngine.Random.Range(-0.8f, 0.8f);
                    float zOffset = UnityEngine.Random.Range(-0.8f, 0.8f);

                    Vector3 pos = new Vector3(x + xOffset, 0, z + zOffset);

                    pos.y = terrain.terrainData.GetInterpolatedHeight(pos.x / terrain.terrainData.size.x,
                        pos.z / (float)terrain.terrainData.size.y);

                    Vector3 eulerRot = new Vector3(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(-180, 180),
                    UnityEngine.Random.Range(-180, 180));


                    float ranScale = Random.Range(0.9f, 2f);
                    GameObject go = Instantiate(prefab, pos, Quaternion.Euler(eulerRot));
                    go.transform.localScale = go.transform.localScale * ranScale;
                    go.transform.SetParent(parent);
                    
                }
            }
        }
    }
/*
    private static int GetCurrentChunk(Vector2Int chunkAmount, Vector2Int pos, Vector2Int size)
    {
        int chunkHeight = size.y / chunkAmount.y;
        int chunkWidth = size.x / chunkAmount.x;
        
        
    }
    */
    private static float Fitness(Terrain terrain, Texture2D noiseMapTexture, int x, int z)
    {
        float fitness = noiseMapTexture.GetPixel(x, z).g;

        float steepness =
            terrain.terrainData.GetSteepness(x / terrain.terrainData.size.x, z / terrain.terrainData.size.z);
        if (steepness < 40)
        {
            fitness -= 1f;
        }

        fitness += Random.Range(-0.25f,0);
        return fitness;
    }
    
}
