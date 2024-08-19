using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using UnityEngine;

namespace SupabaseScripts
{
    [Table("InstantiatedObjects")]
    public class ObjectData : BaseModel
    {
        [PrimaryKey("id")] public string Id { get; set; }
        [Column("created_at")] public DateTimeOffset timeCreated { get; set; }

        [Column("GameKey")] public string GameKey { get; set; }

        //Data
        [Column("ObjectType")] public string ObjectType { get; set; }
        [Column("ObjectData")] public string ObjectInfo { get; set; }

        [Column("Creator")] public string Creator { get; set; }

        //Transform

        [Column("PositionX")] public float PositionX { get; set; }
        [Column("PositionY")] public float PositionY { get; set; }
        [Column("PositionZ")] public float PositionZ { get; set; }

        [Column("RotationX")] public float RotationX { get; set; }
        [Column("RotationY")] public float RotationY { get; set; }
        [Column("RotationZ")] public float RotationZ { get; set; }


        public Vector3 GetPosition()
        {
            return new Vector3(PositionX, PositionY, PositionZ);
        }

        public Vector3 GetRotationEuler()
        {
            return new Vector3(RotationX, RotationY, RotationZ);
        }

        public Quaternion GetRotation()
        {
            return Quaternion.Euler(GetRotationEuler());
        }
    }
}