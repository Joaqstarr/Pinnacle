using System;
using System.Collections;
using Newtonsoft.Json;
using Objects.ObjectTypes;
using SupabaseScripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Objects
{
    public class SpawnableInfo : MonoBehaviour
    {
        [SerializeField] private string _name;
        [SerializeField] private Spawnable _spawnable;
        [SerializeField] private int _amountToPool;
         private SpawnablePool _objectPool;
         [SerializeField] private float _placeableDistance = 10;
         [SerializeField] private float _cooldownInMinutes = 10;
         private float _cooldownTimer;

         private struct SpawnObjectData
         {
             public string ObjectType;
             public Spawnable ObjectToSpawn;
             public Vector3 Pos;
             public Quaternion Rot;
             public string ExtraData;

         }
         private Queue _spawnObjectQueue;
         private void Awake()
         {
             _spawnObjectQueue = new Queue();
             CreatePool();
         }

         private void Update()
         {
             _cooldownTimer += Time.unscaledDeltaTime;

             while (_spawnObjectQueue.Count > 0)
             {
                 SpawnObjectData data = (SpawnObjectData)_spawnObjectQueue.Dequeue();
                 SpawnObject(data.ObjectType, data.Pos, data.Rot, data.ExtraData, data.ObjectToSpawn);

             }
         }

         private void CreatePool()
         {
             GameObject poolObj = new GameObject(_name + "_Pool");
             poolObj.AddComponent<SpawnablePool>();
                
             
             SpawnablePool spawnedPool = poolObj.GetComponent<SpawnablePool>();
             
             spawnedPool.Initialize(_spawnable, _amountToPool);
             _objectPool = spawnedPool;
         }

         public Spawnable GetObject()
         {
             Spawnable toSpawn = null;
             try
             {
                 toSpawn = _objectPool.GetSpawnable();
                 return toSpawn;

             }
             catch (UnityEngine.UnityException exception)
             {
                 Debug.Log("Exception thrown when getting object: " + exception.Message + "\n" + exception.StackTrace);
             }

             return toSpawn;
         }

         public float SpawnRange
         {
             get { return _placeableDistance; }
         }

         public bool CanSpawn()
         {
             return _cooldownTimer > cooldownInSeconds;
         }

         public float cooldownInSeconds
         {
             get
             {
                 return _cooldownInMinutes * 60;
             }
         }

         private void UpdateCooldown(String objType, DateTimeOffset startingTime)
         {
             if (objType.Contains(_name))
             {
                 _cooldownTimer = (float)DateTime.UtcNow.Subtract(startingTime.DateTime).TotalSeconds;
             }
         }

         private void OnSpawnObject(String objType, Vector3 pos, Quaternion rot, String data)
         {
             Debug.Log("Attempt spawn of " + objType + " at pos " + pos + " with other side at " + data);
             if(!objType.Equals(_name))return;

             Spawnable objectSpawned = GetObject();
             
             Debug.Log(objectSpawned.getName);

             SpawnObjectData objectSpawn = new SpawnObjectData
             {
                 ObjectType = objType,
                 Pos = pos,
                 Rot = rot,
                 ObjectToSpawn = objectSpawned,
                 ExtraData = data
             };
             _spawnObjectQueue.Enqueue(objectSpawn);
         }

         private static void SpawnObject(string objType, Vector3 pos, Quaternion rot, string data, Spawnable objectSpawned)
         {
             objectSpawned.Spawn(pos, rot);


             if (objType.Contains("Zipline"))
             {
                 Zipline asZip = (Zipline)objectSpawned;
                 
                 Vector3 otherside = JsonConvert.DeserializeObject<Zipline.SerializableVector>(data).GetValue();
                 asZip.MoveOtherSide(otherside);
                 
             }
         }

         private void OnEnable()
         {
             SupabaseClient.UpdateCooldown += UpdateCooldown;
             SupabaseClient.SpawnObj += OnSpawnObject;
         }

         private void OnDisable()
         {
             SupabaseClient.UpdateCooldown -= UpdateCooldown;
             SupabaseClient.SpawnObj -= OnSpawnObject;

         }
    }
}