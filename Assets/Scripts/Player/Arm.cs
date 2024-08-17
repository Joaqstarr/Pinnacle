using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[System.Serializable]
public class Arm : MonoBehaviour
{
    private Climbable _attachedHold;

    private Transform _parent;
    private SpringJoint _joint;
    private Rigidbody _player;
    [SerializeField] private float _grabDistance = 0.3f;
    [SerializeField] private ChainIKConstraint _ikConstraint;
    // Start is called before the first frame update
    void Start()
    {
        _joint = GetComponent<SpringJoint>();
        _parent = transform.parent;
        _player = transform.root.GetComponent<Rigidbody>();
    }
    

    // Update is called once per frame
    public void AttachToHold(Climbable hold)
    {
        transform.localPosition = Vector3.zero;
        transform.parent = null;
        
        _joint.connectedBody = _player;
        _attachedHold = hold;
        
        
        transform.position = GenerateGrabPosition(_attachedHold.transform.position);
        transform.LookAt(hold.transform.position);
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

    public Vector3 GenerateGrabPosition(Vector3 holdPosition)
    {
        Vector3 toDir = transform.position - holdPosition;

        toDir = toDir.normalized;

        Vector3 offset = toDir * _grabDistance;

        return holdPosition + offset;
    }
}
