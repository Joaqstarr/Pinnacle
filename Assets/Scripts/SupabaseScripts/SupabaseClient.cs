using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Objects;
using Supabase.Gotrue;
using UnityEngine;

namespace SupabaseScripts
{

    public class SupabaseClient
    {
        //Events
        public delegate void UpdateCooldownDel(String objType, DateTime startingTime);

        public static UpdateCooldownDel UpdateCooldown;
        
        //Supabase
        private string _key;
        private string _url;
        private Supabase.Client _supabase;

        //Game
        private string _serverCode;
        private string _userId;
        public SupabaseClient(string key, string url, string serverCode, string userId)
        {
            _key = key;
            _url = url;
            _serverCode = serverCode;
            _userId = userId;

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
            var result = await _supabase.Auth.SignInAnonymously();

            if (_userId.IsNullOrEmpty())
            {
                CreateNewUser(result.User.Id);
            }
            else
            {
                UpdateCooldowns();
            }
            
        }

        private async void UpdateCooldowns()
        {
            var data = await _supabase
                .From<UserRow>()
                .Select("id, zipline: InstantiatedObjects!last_zip(created_at), checkpoint: InstantiatedObjects!last_check(created_at)")
                .Where((x) => (x.Id == _userId))
                .Get();

            
            var result = JsonConvert.DeserializeObject<ObjectCooldownReturn[]>(data.Content);

            Debug.Log(JsonUtility.ToJson(result )+ ", " + data.Content);
            //Debug.Log(result[0].zipline.created_at +",\n" + result[0].checkpoint.created_at);
            
        }
        private async void CreateNewUser(String id)
        {;
            _userId = id;
            PlayerPrefs.SetString("playerid", _userId);

            UserRow userCreated = new UserRow()
            {
                Id = _userId
            };
            
            await _supabase.From<UserRow>().Insert(userCreated);

        }
        public async Task<ObjectData[]> GetObjectData()
        {
            var clientExist = await WaitForClientTimeOut();
            if (!clientExist)
            {
                Debug.LogError("Supabase not initialized yet when attempting fetch");
                
                ObjectData[]objectData = Array.Empty<ObjectData>();
                return Array.Empty<ObjectData>();
                
            }
            
            var results = await _supabase.From<ObjectData>()
                .Where(x => x.GameKey == _serverCode)
                .Get();

            return results.Models.ToArray();
        }

        private async Task<bool> WaitForClientTimeOut()
        {
            int timeOutCount = 0;
            while (_supabase == null)
            {
                await Task.Delay(100);

                
                if (timeOutCount > 20)
                {
                    return false;
                }

                timeOutCount++;
            };
            return true;
        }

        public async void AddObjectToDatabase(Spawnable placedObject){
            var clientExist = await WaitForClientTimeOut();
            if (!clientExist)
            {
                Debug.LogError("Supabase not initialized yet when attempting insert");
                
                return;
                
            }

            Vector3 pos = placedObject.transform.position;
            Vector3 rotEuler = placedObject.transform.eulerAngles;
            var rowToInsert = new ObjectData()
            {
                GameKey = _serverCode,
                ObjectType = placedObject.getName,
                PositionX = pos.x,
                PositionY = pos.y,
                PositionZ = pos.z,

                RotationX = rotEuler.x,
                RotationY = rotEuler.y,
                RotationZ = rotEuler.z,
                Creator = _userId,
                ObjectInfo = placedObject.GetObjectInfo()
            };
            var result = await _supabase.From<ObjectData>().Insert(rowToInsert);
            
            
            

            if (placedObject.name.Contains("Zipline"))
            {
                UpdateLatestZipline(result.Model.Id);    
            }
            if (placedObject.name.Contains("Checkpoint"))
            {
                UpdateLatestCheckpoint(result.Model.Id);    
            }
            
        }

        private void UpdateLatestZipline(string id)
        {
            UpdateCooldown?.Invoke("Zipline", DateTime.Now);

            UserRow updatedInfo = new UserRow()
            {
                Id = _userId,
                lastZip = id
            };
            
            UpdateUserTable(updatedInfo);
            
        }

        private void UpdateLatestCheckpoint(string id)
        {
            UpdateCooldown?.Invoke("Checkpoint", DateTime.Now);

            UserRow updatedInfo = new UserRow()
            {
                Id = _userId,
                lastCheck = id
            };
            UpdateUserTable(updatedInfo);
        }

        private async void UpdateUserTable(UserRow userToUpdate)
        {
            var result = await _supabase.From<UserRow>().Upsert(userToUpdate);
            Debug.Log("updated user " +result.Model.Id + ", " + _userId);
            
        }

        [System.Serializable]
        private class ObjectFoundWithDate
        {
            public DateTime created_at;
        }
        [System.Serializable]
        private class ObjectCooldownReturn
        {
            public string id;
            public ObjectFoundWithDate zipline;
            public ObjectFoundWithDate checkpoint;
        }
    }
}
