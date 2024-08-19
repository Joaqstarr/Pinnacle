using System;
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

         private void UpdateCooldown(String objType, DateTime startingTime)
         {
             if(objType.Contains(_name))
                _cooldownTimer = (float)(DateTime.Now - startingTime).TotalSeconds;
         }

         private void OnEnable()
         {
             
         }

         private void OnDisable()
         {
             
         }
    }
}