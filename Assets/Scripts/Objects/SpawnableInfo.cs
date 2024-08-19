using System;
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
         private void Awake()
         {
             CreatePool();
         }

         private void Update()
         {
             _cooldownTimer += Time.unscaledDeltaTime;
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
             return _objectPool.GetSpawnable();
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
             if(!objType.Contains(_name))return;

             Spawnable objectSpawned = GetObject();
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