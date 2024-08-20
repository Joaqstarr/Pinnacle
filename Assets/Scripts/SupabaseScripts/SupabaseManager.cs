using Objects;
using UnityEngine;

namespace SupabaseScripts
{
    public class SupabaseManager : MonoBehaviour
    {
        public delegate void ObjectDataDel(ObjectData[] objects);

        public static ObjectDataDel ObjectDataReceived;

        [SerializeField] private bool _resetPlayerPrefs;
        
        private SupabaseClient _client;
        private void Start()
        {
            InitializeSupabase();
        }

        private async void InitializeSupabase()
        {
            if (_resetPlayerPrefs)
            {
                PlayerPrefs.SetString("playerid", "");
            }

            string gameKey = PlayerPrefs.GetString("gameKey", "");
            string id = PlayerPrefs.GetString("playerid", "");
            ApiKeys.ApiKeyStruct keys = ApiKeys.PublicGetApiKeys();
            
            _client = new SupabaseClient(keys.GetKey(), keys.GetUrl(), gameKey, id);
            ObjectData[] objectData = await _client.GetObjectData();
            ObjectDataReceived(objectData);
        }

        public void AddObjectToDatabase(Spawnable objectToAdd)
        {
            if (_client != null)
            {
                _client.AddObjectToDatabase(objectToAdd);
            }
        }

        private void OnEnable()
        {
            PlaceObject.ObjectPlaced += AddObjectToDatabase;
        }

        private void OnDisable()
        {
            PlaceObject.ObjectPlaced -= AddObjectToDatabase;
        }
    }
}