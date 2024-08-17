using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Arm : MonoBehaviour
{
    private Climbable _attachedHold;

    private Transform _parent;
    private SpringJoint _joint;
    private Rigidbody _player;
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
        
    }

    public void LetGo()
    {
        transform.parent = _parent;
        transform.localPosition = Vector3.zero;
        _joint.connectedBody = null;

    }

    public bool isAttached => transform.parent == null;
}
