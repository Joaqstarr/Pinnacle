using System;
using DG.Tweening;
using Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AbilityCooldown : MonoBehaviour
    {
        [SerializeField] private SpawnableInfo _connectedInfo;

        [SerializeField] private Image _cooldown;
        private TMP_Text _text;
        private RectTransform _textTransform ;
        private RectTransform _rect;

        private bool _textEnabled = false;
        private Vector3 _startPos;
        
        
        private void Start()
        {

            _text = GetComponentInChildren<TMP_Text>();
            _startPos = getTextRectTransform.anchoredPosition;
            EnableText();
        }

        private void Update()
        {
            if (!_textEnabled)
            {
                if (_connectedInfo.GetTimeRatio() < 1)
                {

                    EnableText();
                }
            }
            else
            {
                if (_connectedInfo.GetTimeRatio() == 1)
                {

                    DisableText();
                }
            }

            _cooldown.fillAmount = 1 - _connectedInfo.GetTimeRatio();
            TimeSpan cooldownProgress = _connectedInfo.GetTimeSpan();
            _text.text = cooldownProgress.Minutes + " : " + cooldownProgress.Seconds;
        }

        private void EnableText()
        {
            _textEnabled = true;


            getTextRectTransform.DOComplete();
            getTextRectTransform.DOLocalMove(_startPos, 0.3f).SetEase(Ease.OutBack);
        }
        private void DisableText()
        {
            _textEnabled = false;


            getTextRectTransform.DOComplete();
            getTextRectTransform.DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
        }

        private RectTransform getTextRectTransform
        {
            get
            {
                if(_textTransform == null)
                    _textTransform = _text.GetComponent<RectTransform>();
                return _textTransform;
            }
            set
            {
                _textTransform = value;
            }
        }
        private RectTransform getRectTransform
        {
            get
            {
                if(_rect == null)
                    _rect = GetComponent<RectTransform>();
                return _rect;
            }
            set
            {
                _rect = value;
            }
        }
        
        private void OnEnable()
        {
            PlaceObject.ObjectEquipped += CheckEquipped;
        }

        private void OnDisable()
        {
            PlaceObject.ObjectEquipped -= CheckEquipped;

        }

        private void CheckEquipped(Spawnable equipped)
        {
            if (_connectedInfo.SpawnablePrefab == equipped)
            {
                getRectTransform.DOComplete();
                getRectTransform.DOScale(Vector3.one * 1.5f, 0.1f);
            }
            else
            {
                getRectTransform.DOComplete();
                getRectTransform.DOScale(Vector3.one, 0.1f);
            }
        }
    }
}