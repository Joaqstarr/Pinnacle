using System;
using System.Collections;
using System.Collections.Generic;
using Objects.ObjectTypes;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CheckpointUi : MonoBehaviour
    {
        private TMP_Text _text;
        
        private void OnEnable()
        {
            _text = GetComponent<TMP_Text>();
            _text.maxVisibleCharacters = 0;

            Checkpoint.NewCheckpointSet += OnCheckpointGet;
        }

        private void OnDisable()
        {
            Checkpoint.NewCheckpointSet -= OnCheckpointGet;
            
        }

        private void OnCheckpointGet(Vector3 pos)
        {
            Debug.Log("checkpoint got " +pos);
            this.StopAllCoroutines();
            StartCoroutine(ShowText());
        }

        IEnumerator ShowText()
        {
            while (_text.maxVisibleCharacters < _text.text.Length)
            {
                _text.maxVisibleCharacters++;
                yield return new WaitForSeconds(0.03f);
            }
            yield return new WaitForSeconds(1f);
            while (_text.maxVisibleCharacters > 0)
            {
                _text.maxVisibleCharacters--;
                yield return new WaitForSeconds(0.03f);
            }
        }
    }
}