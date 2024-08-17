using System;
using System.Collections;
using System.Collections.Generic;
using SupabaseScripts;
using UnityEngine;

namespace Objects
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private ObjectSpawnDataBase[] _spawnableObjects;
        private Dictionary<string, ObjectSpawnDataBase> _spawnableObjectsDict;


        private List<GameObject> _spawnedObjects;
        private void Start()
        {
            _spawnedObjects = new List<GameObject>();
            
            _spawnableObjectsDict = new Dictionary<string, ObjectSpawnDataBase>();
            foreach (var spawnable in _spawnableObjects)
            {
                _spawnableObjectsDict.Add(spawnable.GetObjectName(), spawnable);
            }    
        }

        private void OnEnable()
        {
            SupabaseManager.ObjectDataReceived += OnReceivedFullObjectData;
        }

        private void OnDisable()
        {
            SupabaseManager.ObjectDataReceived -= OnReceivedFullObjectData;
            
        }

        private void OnReceivedFullObjectData(ObjectData[] data)
        {
            foreach (ObjectData objectToSpawn in data)
            {
                string objectType = objectToSpawn.ObjectType;
                Vector3 pos = objectToSpawn.GetPosition();
                Quaternion rot = objectToSpawn.GetRotation();
                string objectInfo = objectToSpawn.ObjectInfo;

                GameObject spawnedObj = _spawnableObjectsDict[objectType].SpawnObject(pos, rot, objectInfo);
                
                _spawnedObjects.Add(spawnedObj);
                
            }
        }
    }
}
