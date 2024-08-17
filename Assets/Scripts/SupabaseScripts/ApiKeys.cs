using System.IO;
using UnityEngine;

namespace SupabaseScripts
{
    public static class ApiKeys
    {
        private static ApiKeyStruct _storedKeys;

        public struct ApiKeyStruct
        {
            private readonly string _supabaseKey;
            private readonly string _supabaseUrl;

            public ApiKeyStruct(string[] fileContents)
            {
                _supabaseKey = fileContents[0];
                _supabaseUrl = fileContents[1];

            }

            public bool IsEmpty()
            {
                return _supabaseKey == "" || _supabaseUrl == "" || _supabaseKey == null || _supabaseUrl == null;
            }

            public string GetKey()
            {
                return _supabaseKey;
            }

            public string GetUrl()
            {
                return _supabaseUrl;
            }
        }
            
        public static ApiKeyStruct PublicGetApiKeys()
        {
            if (_storedKeys.IsEmpty())
            {
                string apiKeyFilePath = ".\\ApiKeys.txt";
                _storedKeys = ReadFileAndCreateStruct(apiKeyFilePath);
            }


            
            
            return _storedKeys;
        }

        private static ApiKeyStruct ReadFileAndCreateStruct(string path)
        {
            string[] keys = File.ReadAllLines(path);

            if (keys[0] == null)
            {
                Debug.LogError("SUPABASE API KEY MISSING");
                return new ApiKeyStruct();
            }
            if (keys[1] == null)
            {
                Debug.LogError("SUPABASE URL KEY MISSING");
                return new ApiKeyStruct();
            }

            return new ApiKeyStruct(keys);
        }
    }
}