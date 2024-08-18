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

         private void Awake()
         {
             CreatePool();
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
    }
}