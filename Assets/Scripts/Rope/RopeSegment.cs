using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    [SerializeField] private const float _minLenght = 0.1f;
    [SerializeField] private Vector3 _endPoint;
    [SerializeField] private Vector3 _startPoint;
    [SerializeField] private RopeSegment _nextSegment;
    [SerializeField] private RopeSegment _previousSegment;
    [SerializeField] private float _lenght;
    [SerializeField] private float _thickness;
    [SerializeField] private bool _isBarrierTouch = false;
    [SerializeField] private bool _startPointLock = false;
    [SerializeField] private bool _endPointLock = false;

    public event Action<RopeSegment> BarrierTouched;
    public event Action<RopeSegment> PreviousSegmentChanged;
    public event Action<RopeSegment> MinimalLenghtReached;

    public Vector3 StartPoint => _previousSegment.EndPoint;
    public Vector3 EndPoint => _endPoint;
    public RopeSegment PreviousSegment => _previousSegment;

    public RopeSegment NextSegment => _nextSegment;

    public bool IsBarrierTouch => _isBarrierTouch;


    private void Update()
    {
        CheckNeiboursSegments();
        if (!_isBarrierTouch)
        {
            Decrease();
            UpdateState();

        }

    }

    private void CheckNeiboursSegments()
    {
        _startPointLock = _previousSegment == null ? true : _previousSegment.IsBarrierTouch;
        _endPointLock = _nextSegment == null ? true : _nextSegment.IsBarrierTouch;

    }

    private void Decrease()
    {
        if (_lenght > _minLenght)
        {
            if (!_endPointLock)
                _endPoint = Vector3.MoveTowards(_endPoint, _startPoint, Time.deltaTime * 10f);
            else if (!_startPointLock)
                _startPoint = Vector3.MoveTowards(_startPoint, _endPoint, Time.deltaTime * 10f);
        }
    }

    public void Init(Vector3 endPoint, RopeSegment previousSegment, float thickness, Rope rope)
    {
        _endPoint = endPoint;
        ChangePreviousSegment(previousSegment);
        _thickness = thickness;
        //_previousSegment.ChangeNextSegment(this);

        BarrierTouched += rope.SegmentDivision;
        MinimalLenghtReached += rope.DeleteSegment;
        //UpdateState();
    }

    public void UpdateState()
    {
        RefreshPosition();
        RefreshLenght();
        RefreshRotation();
    }

    private void RefreshPosition()
    {
        _startPoint = _previousSegment.EndPoint;

        if (_endPointLock)
        {
            _endPoint = _nextSegment.StartPoint;
        }

        transform.position = _startPoint;
    }

    private void RefreshLenght()
    {
        float lenght = Vector3.Distance(_endPoint, _startPoint);

        if (lenght > _minLenght)
        {
            _lenght = lenght;
        }
        else
        {
            _lenght = _minLenght;

            if (!_isBarrierTouch && !_startPointLock && !_endPointLock)
            {
                MinimalLenghtReached?.Invoke(this);
            }
        }

        transform.localScale = new Vector3(_thickness, _lenght, _thickness);
    }

    private void RefreshRotation()
    {
        if (_nextSegment != null)
        {
            transform.LookAt(_nextSegment.StartPoint, Vector3.up);
        }

        transform.Rotate(90, 0, 0);
    }

    public void ChangePreviousSegment(RopeSegment previousSegment)
    {
        //if (_previousSegment != previousSegment&&_previousSegment!=null)
        //{
        //    PreviousSegmentChanged?.Invoke(previousSegment);
        //_previousSegment.ChangeNextSegment(previousSegment);            
        //}

        _previousSegment = previousSegment;
        _previousSegment.ChangeNextSegment(this);
        //PreviousSegmentChanged = previousSegment.ChangeNextSegment;
    }

    public void ChangeNextSegment(RopeSegment nextSegment)
    {
        _nextSegment = nextSegment;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<Barrier>(out Barrier barrier))
        {
            Debug.Log("touched");

            if (_lenght > _minLenght)
            {
                BarrierTouched?.Invoke(this);
            }
            
            _isBarrierTouch = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.TryGetComponent<Barrier>(out Barrier barrier))
        {
            _isBarrierTouch = false;
        }
    }
}
