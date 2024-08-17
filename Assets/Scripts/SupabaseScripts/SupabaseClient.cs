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

            CreateClient();

        }

        private async void CreateClient()
        {
            Supabase.SupabaseOptions options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            Supabase.Client supabase = new Supabase.Client(_url, _key, options);
            await supabase.InitializeAsync();
            _supabase = supabase;
        }

        public async void GetObjectData()
        {
            if (_supabase == null)
            {
                Debug.LogWarning("Supabase client not created yet when attempting object retrieval.");
                return;
            }

            var results = await _supabase.From<ObjectData>()
                .Where(x => x.GameKey != _serverCode)
                .Get();

            Debug.LogWarning(results);
        }


    }
}
