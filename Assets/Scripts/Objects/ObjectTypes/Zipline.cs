using Newtonsoft.Json;
using UnityEngine;

namespace Objects.ObjectTypes
{
    public class Zipline : Spawnable
    {
        [SerializeField] private Transform _otherSide;
        [SerializeField] private Transform _rope;

        public override void Place(RaycastHit hit, Vector3 playerLocation)
        {
            base.Place(hit, playerLocation);
            
            MoveOtherSide(playerLocation);

        }

        public void MoveOtherSide(Vector3 newPos)
        {
            _otherSide.position = newPos;

            _rope.position = (newPos + transform.position) / 2;
            
            _rope.LookAt(newPos);

            Vector3 newScale = _rope.transform.localScale;
            newScale.z = Vector3.Distance(newPos, transform.position)/2;
            _rope.transform.localScale = newScale;
        }
        
        public override string GetObjectInfo()
        {
            return JsonConvert.SerializeObject(new SerializableVector(_otherSide.transform.position));
        }

        [System.Serializable]
        public class SerializableVector
        {
            public float x;
            public float y;
            public float z;

            public Vector3 GetValue()
            {
                return new Vector3(x, y, z);
            }

            public SerializableVector(Vector3 pos)
            {
                x = pos.x;
                y = pos.y;
                z = pos.z;
            }
        }
    }
}