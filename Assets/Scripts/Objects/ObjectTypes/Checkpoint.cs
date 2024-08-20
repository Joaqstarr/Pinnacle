using System;
using Player;
using UnityEngine;

namespace Objects.ObjectTypes
{
    public class Checkpoint : MonoBehaviour
    {
        public delegate void NewCheckpointDel(Vector3 pos);

        public static NewCheckpointDel NewCheckpointSet;
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (other.GetComponent<PlayerBrain>() != null)
            {
                NewCheckpointSet?.Invoke(transform.position + new Vector3(0, 1.5f, 0));
            }
        }
    }
}