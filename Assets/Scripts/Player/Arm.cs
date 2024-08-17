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

    [SerializeField] private ChainIKConstraint _ikConstraint;
    // Start is called before the first frame update
    void Start()
    {
        _joint = GetComponent<SpringJoint>();
        _parent = transform.parent;
        _player = transform.root.GetComponent<Rigidbody>();
    }

    private void Update()
    {
       // _joint.connectedAnchor = transform.InverseTransformPoint(_player.transform.position);
    }

    // Update is called once per frame
    public void AttachToHold(Climbable hold)
    {
        transform.parent = null;
        _joint.connectedBody = _player;
        _attachedHold = hold;
        transform.position = _attachedHold.transform.position;
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
}
