using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;

public class BuildScreen : MonoBehaviour
{

    [SerializeField] private PlayerBrain _player;
    private CanvasGroup _group;

    private bool _enabled = false;
    private RectTransform _rect;

    private Vector3 _startPos;

    private Vector3 _hiddenPos;
    // Start is called before the first frame update
    void Start()
    {
        
        _rect = GetComponent<RectTransform>();
        _group = GetComponent<CanvasGroup>();

        _startPos = _rect.anchoredPosition;
        _hiddenPos = _startPos + new Vector3(0, -319, 0);
        _rect.anchoredPosition = _hiddenPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (_player.isBuildModeEnabled && _enabled == false)
        {
            EnableMenu();
        }
        
        if(!_player.isBuildModeEnabled && _enabled)
        {
            DisableMenu();
        }
    }

    private void EnableMenu()
    {
        _enabled = true;
        _rect.DOComplete();
        _rect.DOMove(_startPos, 0.3f).SetEase(Ease.OutBack);
    }

    private void DisableMenu()
    {
        _enabled = false;
        _rect.DOComplete();

        _rect.DOMove(_hiddenPos, 0.3f).SetEase(Ease.InBack);
        
    }
}
