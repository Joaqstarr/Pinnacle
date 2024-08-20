using System;
using UnityEngine;

namespace Environment
{
    public class BuildBlocker : MonoBehaviour
    {
        private Collider[] _blockingColliders;

        public static BuildBlocker Instance;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

            _blockingColliders = GetComponentsInChildren<Collider>();
        }

        public bool IsBlocked(Vector3 pos)
        {
            foreach (var collider in _blockingColliders)
            {
                if (collider.bounds.Contains(pos))
                    return true;
            }

            return false;
        }
    }
}