using System;
using DG.Tweening;
using Player;
using UnityEngine;

namespace Music
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private PlayerBrain _player;

        [SerializeField] private AudioSource _climbTrack;
        [SerializeField] private MusicHeightData[] _tracks;
        [System.Serializable]
        private struct MusicHeightData
        {
            public float _height;
            public AudioSource _music;
            
            
            public void UpdateVolume(float height)
            {
                bool shouldBeEnabled = height > _height;

                if (shouldBeEnabled)
                {
                    
                    _music.volume = Mathf.Lerp(_music.volume, 1, Time.deltaTime);
                }
                if (!shouldBeEnabled)
                {
                    
                    _music.volume = Mathf.Lerp(_music.volume, 0, Time.deltaTime);
                }
            }
        }

        private void Update()
        {
            UpdateVolume();
        }

        private void UpdateVolume()
        {
            for (int i = 0; i < _tracks.Length; i++)
            {
                _tracks[i].UpdateVolume(_player.transform.position.y);
            }

            bool shouldBeEnabled = _player.isClimbing;

            if (shouldBeEnabled)
            {
                    
                _climbTrack.volume = Mathf.Lerp(_climbTrack.volume, 1, Time.deltaTime * 0.05f);
            }
            if (!shouldBeEnabled)
            {
                    
                _climbTrack.volume = Mathf.Lerp(_climbTrack.volume, 0, Time.deltaTime);
            }
        }
    }
}