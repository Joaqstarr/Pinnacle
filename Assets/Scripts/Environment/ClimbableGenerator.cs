using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class ClimbableGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject _climbablePrefab;

        private Terrain _terrain;
        // Start is called before the first frame update
        void Start()
        {
            _terrain = GetComponent<Terrain>();
            TreeInstance[] trees = _terrain.terrainData.treeInstances;
            SpawnClimbables(trees);
        }

        private void SpawnClimbables(TreeInstance[] trees)
        {
            if(_climbablePrefab == null) return;
            
            foreach (var tree in trees)
            {
                GameObject spawnedObject =
                    GameObject.Instantiate<GameObject>(_climbablePrefab, tree.position * 3000, Quaternion.identity, transform);
            }
        }
    }
}

