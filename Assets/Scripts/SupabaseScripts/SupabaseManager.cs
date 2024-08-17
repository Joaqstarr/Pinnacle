using System;
using UnityEngine;

namespace SupabaseScripts
{
    public class SupabaseManager : MonoBehaviour
    {
        public delegate void ObjectDataDel(ObjectData[] objects);

        public static ObjectDataDel ObjectDataReceived;
            
        
        private SupabaseClient _client;
        private void Start()
        {
            InitializeSupabase();
        }

        private async void InitializeSupabase()
        {
            ApiKeys.ApiKeyStruct keys = ApiKeys.PublicGetApiKeys();
            
            _client = new SupabaseClient(keys.GetKey(), keys.GetUrl(), "");
            ObjectData[] objectData = await _client.GetObjectData();
            ObjectDataReceived(objectData);
        }
    }
}