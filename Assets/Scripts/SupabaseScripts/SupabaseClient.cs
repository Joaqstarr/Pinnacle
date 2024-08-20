using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Objects;
using Supabase.Gotrue;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.PostgresChanges;
using Unity.VisualScripting;
using UnityEngine;
using Constants = Supabase.Realtime.Constants;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SupabaseScripts
{

    public class SupabaseClient
    {
        //Events
        public delegate void UpdateCooldownDel(String objType, DateTimeOffset startingTime);
        public delegate void SpawnObjectDel(String objType, Vector3 pos, Quaternion rot ,String data);

        public static SpawnObjectDel SpawnObj;
        
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
            SubscribeToObjectStream();

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
            //await CreateNewUser(result.User.Id);
            if (_userId.IsNullOrEmpty())
            {
                CreateNewUser(result.User.Id);
                //CreateNewUser(result.User.Id);

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

            string modifiedContent = data.Content.Substring(1, data.Content.Length - 2);
            if(modifiedContent.IsNullOrEmpty())return;
            var jObject = JObject.Parse(modifiedContent);
            var zipline = JsonConvert.DeserializeObject<ObjectFoundWithDate>(jObject["zipline"].ToString());
            if (zipline != null)
            {
                UpdateCooldown?.Invoke("Zipline", zipline.created_at);
            }
            var checkpoint = JsonConvert.DeserializeObject<ObjectFoundWithDate>(jObject["checkpoint"].ToString());
            if (checkpoint != null)
            {
                UpdateCooldown?.Invoke("Checkpoint", checkpoint.created_at);
            }

        }
        private async Task<string> CreateNewUser(String id)
        {;
            _userId = id;
            PlayerPrefs.SetString("playerid", _userId);

            UserRow userCreated = new UserRow()
            {
                Id = id
            };
            
            
            await _supabase.From<UserRow>().Insert(userCreated);
            return _userId;

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
                timeCreated = DateTimeOffset.UtcNow,
                ObjectType = placedObject.getName,
                PositionX = pos.x,
                PositionY = pos.y,
                PositionZ = pos.z,

                RotationX = rotEuler.x,
                RotationY = rotEuler.y,
                RotationZ = rotEuler.z,
                Creator = _userId,
                //Creator = _supabase.Auth.CurrentUser.Id,
                ObjectInfo = placedObject.GetObjectInfo()
            };
            Debug.Log(_userId + ", " + _supabase.Auth.CurrentUser.Id);
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
            UpdateCooldown?.Invoke("Zipline", DateTime.UtcNow);

            UserRow updatedInfo = new UserRow()
            {
                Id = _userId,
                lastZip = id
            };
            
            UpdateUserTable(updatedInfo);
            
        }

        private void UpdateLatestCheckpoint(string id)
        {
            UpdateCooldown?.Invoke("Checkpoint", DateTime.UtcNow);

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
            [JsonProperty("created_at")]
            [SerializeField]public DateTimeOffset created_at{ get; set; }
        }
        [System.Serializable]
        private class ObjectCooldownReturn
        {
            [JsonProperty("id")]
            public string id;
            [JsonProperty("zipline")]
            public ObjectFoundWithDate zipline;
            [JsonProperty("checkpoint")]
            public ObjectFoundWithDate checkpoint;
        }

        private async void SubscribeToObjectStream()
        {
            var clientExist = await WaitForClientTimeOut();
            if (!clientExist)
            {
                Debug.LogError("Supabase not initialized yet when attempting subscription");
                
                return;
                
            }

            var channel = _supabase.Realtime.Channel("realtime", "public", "InstantiatedObjects");

            await _supabase.From<ObjectData>().On(PostgresChangesOptions.ListenType.Inserts, (sender, change) =>
            {
                  

                ObjectData objToSpawn = change.Model<ObjectData>();
                Debug.Log("TABLE CHANGED " +objToSpawn.ObjectType);

                SpawnObj?.Invoke(objToSpawn.ObjectType, objToSpawn.GetPosition(), objToSpawn.GetRotation(),
                    objToSpawn.ObjectInfo);
            });
            
            await channel.Subscribe();
            Debug.Log("subscribed");
            
        }


    }
}
