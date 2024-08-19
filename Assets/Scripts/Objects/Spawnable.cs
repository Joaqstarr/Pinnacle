using UnityEngine;

namespace Objects
{
    public class Spawnable : MonoBehaviour
    {
        [SerializeField] private string _name;
        [SerializeField] private float _distanceFromGround;
        private bool _isSpawned = false;
        public void PreviewLocation(RaycastHit hit)
        {
            if (!gameObject.activeSelf)
            {
                _isSpawned = true;
                gameObject.SetActive(true);
            }
            transform.position = CalculatePositionWithOffsetFromNormal(hit.point, hit.normal);
            transform.LookAt(hit.point);
        }

        public virtual void Place(RaycastHit hit, Vector3 playerLocation)
        {
            PreviewLocation(hit);
            
            Spawn(transform.position, transform.rotation);
        }

        public void Spawn(Vector3 location, Quaternion rotation)
        {
            transform.position = location;
            transform.rotation = transform.rotation;
            _isSpawned = true;
            gameObject.SetActive(true);
            
            //todo enable collisions and functions
        }

        public void DeSpawn()
        {
            _isSpawned = false;
            gameObject.SetActive(false);
        }

        private Vector3 CalculatePositionWithOffsetFromNormal(Vector3 position, Vector3 normal)
        {
            return position + (normal * _distanceFromGround);
        }

        public string getName => _name;
        public virtual string GetObjectInfo()
        {
            return "";
        }

        public bool Active
        {
            get
            {
                return _isSpawned;
            }
        }
        
    }
}