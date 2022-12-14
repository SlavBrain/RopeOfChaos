using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    [SerializeField] private const float _minLenght = 0.2f;
    [SerializeField] private float _decreasingSpeed = 0.1f;
    [SerializeField] private Vector3 _startPointCoordinate;
    [SerializeField] private EndSegmentDot _endPoint;
    [SerializeField] private RopeSegment _nextSegment;
    [SerializeField] private RopeSegment _previousSegment;
    [SerializeField] private float _lenght;
    [SerializeField] private float _thickness;
    [SerializeField] private float _angle;

    [SerializeField] private bool _isBarrierTouch = false;
    [SerializeField] private bool _isStartPointLock = false;
    [SerializeField] private bool _isEndPointLock = false;
    [SerializeField] private bool _isNeibourSegmentOneSideLock = false;

    public event Action<RopeSegment> BarrierTouched;
    public event Action<RopeSegment> PreviousSegmentChanged;
    public event Action<RopeSegment> ReachedDeleatingState;

    public RopeSegment PreviousSegment => _previousSegment;
    public RopeSegment NextSegment => _nextSegment;
    public EndSegmentDot EndPoint => _endPoint;

    public bool IsBarrierTouch => _isBarrierTouch;
    public bool IsEndPointLock => _isEndPointLock;
    public bool IsStartPointLock => _isStartPointLock;

    private void Awake()
    {
        _endPoint = GetComponentInChildren<EndSegmentDot>();
    }

    private void Update()
    {
        if (_previousSegment != null && _nextSegment != null)
        {
            CheckNeibourSegments();

            if (_isBarrierTouch)
            {
                TouchingStateUpdate();
            }
            else if (_isStartPointLock || _isEndPointLock)
            {
                if (_isStartPointLock)
                {
                    if (_isEndPointLock)
                    {
                        LockAroundStateUpdate();
                    }
                    else
                    {
                        StartPointLockStateUpdate();
                    }
                }
                else
                {
                    EndPointLockStateUpdate();
                }
            }
            else if (_nextSegment.IsEndPointLock || _previousSegment.IsStartPointLock)
            {
                LockOneSideNeibourSegmentStateUpdate();
            }
            else
            {
                FreeStateUpdate();

                if (!_isBarrierTouch && !_isStartPointLock && !_isEndPointLock)
                {
                    ReachedDeleatingState?.Invoke(this);
                }
            }            
        }
    }

    private void TouchingStateUpdate()
    {
        DecreaseToStartPoint();
    }

    private void LockAroundStateUpdate()
    {
        //nothing 
    }

    private void StartPointLockStateUpdate()
    {
        //decrease endPoint=>startPoint
        DecreaseToStartPoint();
    }

    private void EndPointLockStateUpdate()
    {
        //decrease startPoint=>endPoint
        DecreaseToEndPoint();
    }

    private void LockOneSideNeibourSegmentStateUpdate()
    {
        //1)startPoint=prevSegm.end
        transform.position = _previousSegment.EndPoint.transform.position;
        //2)lookAt end
        SetDirectToEndPoint();
        //3)size Y == start,_next.Start lenght
        transform.localScale = new Vector3(_thickness, Vector3.Distance(transform.position, _nextSegment.transform.position), _thickness);
    }

    private void FreeStateUpdate()
    {
        //decrease start=>end 
        DecreaseToEndPoint();
        //position start=_prev.End
        SetPosition();
        //Rotate around Start  lookat end
        SetDirectToEndPoint();
    }

    public void Init(RopeSegment nextSegment, float thickness, Rope rope)
    {
        _thickness = thickness;
        ChangeNextSegment(nextSegment);
        transform.Rotate(90, 0, 0);
        ReCalculateLenght();
        BarrierTouched += rope.SegmentDivision;
        ReachedDeleatingState += rope.DeleteSegment;
    }

    private void CheckNeibourSegments()
    {
        _isStartPointLock = _previousSegment.IsBarrierTouch;
        _isEndPointLock = _nextSegment.IsBarrierTouch;
    }

    private void DecreaseToStartPoint()
    {
        _lenght = Mathf.MoveTowards(_lenght, _minLenght, Time.deltaTime * _decreasingSpeed);
        transform.localScale = new Vector3(_thickness, _lenght, _thickness);
    }

    private void DecreaseToEndPoint()
    {
        DecreaseToStartPoint();
        transform.position = new Vector3(transform.position.x + (_nextSegment.transform.position.x - _endPoint.transform.position.x),
                                         transform.position.y,
                                         transform.position.z + (_nextSegment.transform.position.z - _endPoint.transform.position.z));
    }

    private void SetDirectToEndPoint()
    {
        transform.LookAt(_nextSegment.transform.position);
        transform.Rotate(90, 0, 0);
    }

    private void SetPosition()
    {
        transform.position = _previousSegment.EndPoint.transform.position;
    }

    private void ReCalculateLenght()
    {
        float lenght = Vector3.Distance(transform.position, _nextSegment.transform.position);

        if (lenght > _minLenght)
        {
            _lenght = lenght;
        }
        else
        {
            _lenght = _minLenght;

            if (!_isBarrierTouch && !_isStartPointLock && !_isEndPointLock)
            {
                ReachedDeleatingState?.Invoke(this);
            }
        }

        transform.localScale = new Vector3(_thickness, _lenght, _thickness);
    }


    public void ChangePreviousSegment(RopeSegment previousSegment)
    {
        _previousSegment = previousSegment;
    }

    public void ChangeNextSegment(RopeSegment nextSegment)
    {
        _nextSegment = nextSegment;
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
