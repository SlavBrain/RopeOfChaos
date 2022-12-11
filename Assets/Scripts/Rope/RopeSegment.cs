using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FixedJoint))]
public class RopeSegment : MonoBehaviour
{
    [SerializeField] private const float _minLenght = 0.1f;
    [SerializeField] private Vector3 _startPointCoordinate;
    [SerializeField] private StartSegmentDot _startPoint;
    [SerializeField] private readonly EndSegmentDot _endPoint;
    [SerializeField] private RopeSegment _nextSegment;
    [SerializeField] private RopeSegment _previousSegment;
    [SerializeField] private float _lenght;
    [SerializeField] private float _thickness;
    [SerializeField] private bool _isBarrierTouch = false;
    [SerializeField] private bool _startPointLock = false;
    [SerializeField] private bool _endPointLock = false;
    [SerializeField] private float _angle;

    private Rigidbody _rigidbody;
    private FixedJoint _fixedJoint;

    public event Action<RopeSegment> BarrierTouched;
    public event Action<RopeSegment> PreviousSegmentChanged;
    public event Action<RopeSegment> ReachedDeleatingState;
    public RopeSegment PreviousSegment => _previousSegment;

    public RopeSegment NextSegment => _nextSegment;

    public bool IsBarrierTouch => _isBarrierTouch;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _fixedJoint = GetComponent<FixedJoint>();
        _startPoint = GetComponentInChildren<StartSegmentDot>();
        _endPoint = GetComponentInChildren<EndSegmentDot>();
    }

    private void Update()
    {
        //CheckNeiboursSegments();

        //if (!_isBarrierTouch)
        //{
        //    Decrease();
        //    

        //}

        UpdateState();
    }

    public void Init(RopeSegment nextSegment, Vector3 startPointCoordinate, float thickness, Rope rope)
    {
        _startPointCoordinate = startPointCoordinate;
        _thickness = thickness;
        ChangeNextSegment(nextSegment);
        transform.Rotate(90, 0, 0);
        //BarrierTouched += rope.SegmentDivision;
        //ReachedDeleatingState += rope.DeleteSegment;

        //UpdateState();
    }

    public void UpdateState()
    {
        //RefreshPosition();
        //RefreshLenght();
        //RefreshRotation();
    }

    private void RefreshPosition()
    {

    }

    private void RefreshLenght()
    {
        float lenght = Vector3.Distance(_startPoint.transform.position, _endPoint.transform.position);

        if (lenght > _minLenght)
        {
            _lenght = lenght;
        }
        else
        {
            _lenght = _minLenght;

            if (!_isBarrierTouch && !_startPointLock && !_endPointLock)
            {
                //ReachedDeleatingState?.Invoke(this);
            }
        }

        transform.localScale = new Vector3(_thickness, _lenght, _thickness);
    }

    private void RefreshRotation()
    {
    }

    public void ChangePreviousSegment(RopeSegment previousSegment)
    {
        _previousSegment = previousSegment;
    }

    public void ChangeNextSegment(RopeSegment nextSegment)
    {
        _nextSegment = nextSegment;
        //_fixedJoint.connectedBody = _nextSegment.GetComponent<Rigidbody>();
        _nextSegment.ChangePreviousSegment(this);
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
