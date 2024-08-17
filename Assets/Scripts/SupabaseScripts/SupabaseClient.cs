using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace SupabaseScripts
{
    public class SupabaseClient
    {
        //Supabase
        private string _key;
        private string _url;
        private Supabase.Client _supabase;

        //Game
        private string _serverCode;

        public SupabaseClient(string key, string url, string serverCode)
        {
            _key = key;
            _url = url;
            _serverCode = serverCode;

            InitializeSupabase();
        }

        private async void InitializeSupabase()
        {
            await CreateClient();
            SignInAnonymously();
        }

        private async Task<bool> CreateClient()
        {
            Supabase.SupabaseOptions options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            Supabase.Client supabase = new Supabase.Client(_url, _key, options);
            await supabase.InitializeAsync();
            _supabase = supabase;

            return true;
        }

        private async void SignInAnonymously()
        {
            if (_supabase == null)
            {
                Debug.LogError("Supabase not initialized yet when attempting to sign in anonymously");
                return;
            }

            await _supabase.Auth.SignInAnonymously();
        }
        public async Task<ObjectData[]> GetObjectData()
        {
            int timeOutCount = 0;
            while (_supabase == null)
            {
                await Task.Delay(100);

                
                if (timeOutCount > 20)
                {
                    Debug.LogError("Supabase not initialized yet when attempting fetch");
                    return Array.Empty<ObjectData>();
                }

                timeOutCount++;
            };

            var results = await _supabase.From<ObjectData>()
                .Where(x => x.GameKey == _serverCode)
                .Get();

            return results.Models.ToArray();
        }

        
    }
}
