using System.Collections;
using System.Collections.Generic;
using Objects;
using Player;
using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    public delegate void PlaceObjectDel(Spawnable objectPlaced);

    public static PlaceObjectDel ObjectPlaced;
    
    
    [SerializeField]private SpawnableInfo _equipped;
    private PlayerControls _controls;

    private Transform _headPos;

    private PlayerData _data;

    private Spawnable _objectToPlace;
    private bool _buildMode = false;
    public void Initialize(PlayerData data, PlayerControls controls,Transform headPos)
    {
        _headPos = headPos;
        _data = data;
        _controls = controls;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_buildMode)
        {
            PreviewObject();
        }
    }

    public void EnterBuildMode()
    {
        _controls.PlacePressed += TryPlaceObject;
        _objectToPlace = null;
        _buildMode = true;
    }

    public void ExitBuildMode()
    {
        _buildMode = false;
        _controls.PlacePressed -= TryPlaceObject;
        if (_objectToPlace)
        {
            _objectToPlace.DeSpawn();
            _objectToPlace = null;
        }
    }

    public void PreviewObject()
    {
        if (_objectToPlace == null)
        {
            _objectToPlace = _equipped.GetObject();
        }
        Debug.DrawRay(_headPos.position, _headPos.forward * _equipped.SpawnRange, Color.cyan);

        if(!IsValidLocation(out RaycastHit hit))
        {
            return;
        }

        
       _objectToPlace.PreviewLocation(hit);
        
    }

    public void TryPlaceObject()
    {
        if(!IsValidLocation(out RaycastHit hit))
        {
            return;
        }
        _objectToPlace.Place(hit);
        ObjectPlaced?.Invoke(_objectToPlace);
        _objectToPlace = null;
    }

    private bool IsValidLocation(out RaycastHit hit)
    {
        return Physics.Raycast(_headPos.position, _headPos.forward, out hit, _equipped.SpawnRange,
            _data.groundLayers);
    }
}
