using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandAnimator : MonoBehaviour
{
    [System.Serializable]
    struct FingerInfo
    {
        public ChainIKConstraint ik;
        public Vector3 defaultPos;
        public Transform target;
    }
    
    [SerializeField] private FingerInfo[] _fingers;
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private LayerMask _fingerBlockingLayers;

    private void Awake()
    {
        for (int i = 0; i < _fingers.Length; i++)
        {
            _fingers[i].defaultPos = transform.InverseTransformPoint(_fingers[i].ik.data.tip.transform.position);
            GameObject obj = new GameObject("fingerTarget");
            GameObject spawnedObject = GameObject.Instantiate(obj, transform.TransformPoint(_fingers[i].defaultPos), quaternion.identity, transform);

            _fingers[i].ik.data.target = spawnedObject.transform;
            _fingers[i].target = spawnedObject.transform;
        }
    }

    private void Update()
    {
        UpdateAllFingers();
    }

    private void UpdateAllFingers()
    {
        for (int i = 0; i < _fingers.Length; i++)
        {
            UpdateFingerIkPos(_fingers[i]);
        }
    }
    private void UpdateFingerIkPos(FingerInfo finger)
    {
        Vector3 dir = _holdPoint.position - transform.TransformPoint(finger.defaultPos);
        Vector3 dirNormalized = dir.normalized;
        float distance = dir.magnitude;
        Debug.DrawRay(transform.TransformPoint(finger.defaultPos), dir, Color.red);
        if (Physics.Raycast( transform.TransformPoint(finger.defaultPos), dirNormalized, out RaycastHit hit, distance, _fingerBlockingLayers))
        {
            finger.target.transform.position = hit.point;
        }
        else
        {
            finger.target.transform.position = transform.TransformPoint(finger.defaultPos);
        }
    }
    
}
