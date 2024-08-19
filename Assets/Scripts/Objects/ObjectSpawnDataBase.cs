using Newtonsoft.Json;
using Objects.ObjectTypes;
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

            if (_objectName.Equals("Zipline"))
            {
                Vector3 otherside = JsonConvert.DeserializeObject<Zipline.SerializableVector>(objectData).GetValue();
                instantiatedObj.GetComponent<Zipline>().MoveOtherSide(otherside);
            }
            
            
            return instantiatedObj;
        }


        public string GetObjectName()
        {
            return _objectName;
        }
    }
}