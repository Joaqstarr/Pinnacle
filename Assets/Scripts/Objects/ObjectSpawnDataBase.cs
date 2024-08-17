using UnityEngine;

namespace Objects
{
    [CreateAssetMenu( menuName = "ObjectSpawnDataBase")]
    public class ObjectSpawnDataBase : ScriptableObject
    {
        [SerializeField] private string _objectName;
        [SerializeField] private GameObject _objectPrefab;

        public GameObject SpawnObject(Vector3 position, Quaternion rotation,  string objectData)
        {
            GameObject instantiatedObj = GameObject.Instantiate(_objectPrefab, position, rotation, null);
            return instantiatedObj;
        }


        public string GetObjectName()
        {
            return _objectName;
        }
    }
}