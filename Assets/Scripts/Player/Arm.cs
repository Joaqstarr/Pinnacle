using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

[System.Serializable]
public class Arm : MonoBehaviour
{
    private Climbable _attachedHold;

    private Transform _parent;
    private SpringJoint _joint;
    private Rigidbody _player;
    [SerializeField] private float _grabDistance = 0.3f;
    [SerializeField] private ChainIKConstraint _ikConstraint;

    private AudioSource _audioSource;

    [SerializeField] private AudioClip[] _grabSounds;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _joint = GetComponent<SpringJoint>();
        _parent = transform.parent;
        _player = transform.root.GetComponent<Rigidbody>();
    }
    

    // Update is called once per frame
    public void AttachToHold(PlayerBrain.ClimbableData holdData)
    {
        PlayGrabSound();
        transform.localPosition = Vector3.zero;
        transform.parent = null;
        
        _joint.connectedBody = _player;
        _attachedHold = holdData.hold;
        
        
        transform.position = GenerateGrabPosition(holdData.grabPosition, holdData.normalDir);
        transform.LookAt(holdData.grabPosition);
        DOVirtual.Float(_ikConstraint.weight, 1, 0.4f, (x) => { _ikConstraint.weight = x; }).SetEase(Ease.OutCirc);
    }

    public void LetGo()
    {
        transform.parent = _parent;
        //transform.localPosition = Vector3.zero;
        _joint.connectedBody = null;

        DOVirtual.Float(_ikConstraint.weight, 0, 0.4f, (x) => { _ikConstraint.weight = x; }).SetEase(Ease.OutCirc);

    }

    public bool isAttached => transform.parent == null;

    public Vector3 GenerateGrabPosition(Vector3 holdPosition, Vector3 dir)
    {
        Vector3 toDir = transform.position - holdPosition;

        toDir = toDir.normalized;


        toDir = dir;
        Vector3 offset = toDir * _grabDistance;

        return holdPosition + offset;
    }

    private void PlayGrabSound()
    {
        AudioClip sound = _grabSounds[Random.Range(0, _grabSounds.Length)];

        _audioSource.pitch = Random.Range(1.5f, 2.0f);
        _audioSource.PlayOneShot(sound,Random.Range(0.8f, 1.2f));
    }
}
