using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class SpawnablePool : MonoBehaviour
    {
        [SerializeField] private Spawnable _objectToPool;
        private List<Spawnable> _pool = new List<Spawnable>();

        [SerializeField] private int _amountToPool = 100;
        public void Initialize(Spawnable objectToPool, int amt = 100)
        {
            _amountToPool = amt;
            _objectToPool = objectToPool;

            if(_objectToPool == null )return;

            for (int i = 0; i < _amountToPool; i++)
            {
                CreateObject();
            }
        }

        private Spawnable CreateObject()
        {
            Spawnable objInstantiated = Instantiate(_objectToPool, transform);
            objInstantiated.DeSpawn();
            _pool.Add(objInstantiated);
            return objInstantiated;
        }

        public Spawnable GetSpawnable()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                bool active = _pool[i].Active;
                if (!active)
                {
                    Debug.Log(i);
                    return _pool[i];
                }
            }

            return CreateObject();
        }
    }
}