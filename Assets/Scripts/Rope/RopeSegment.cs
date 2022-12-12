using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    [SerializeField] private const float _minLenght = 0.1f;
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

    public Vector3 StartPointCoordinate => _startPointCoordinate;

    public bool IsBarrierTouch => _isBarrierTouch;
    public bool IsEndPointLock => _isEndPointLock;
    public bool IsStartPointLock => _isStartPointLock;

    private void Awake()
    {
        _endPoint = GetComponentInChildren<EndSegmentDot>();
    }

    private void Update()
    {
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
        }
    }

    private void TouchingStateUpdate()
    {
        //Divine and position Normal barrier
    }

    private void LockAroundStateUpdate()
    {
        //nothing 
    }

    private void StartPointLockStateUpdate()
    {
        //decrease endPoint=>startPoint
    }

    private void EndPointLockStateUpdate()
    {
        //decrease startPoint=>endPoint
    }

    private void LockOneSideNeibourSegmentStateUpdate()
    {
        //startPoint=prevSegm.end and endPoint=nextSeg.start
        //1)startPoint=prevSegm.end
        //2)lookAt end
        //3)size Y == start,_next.Start lenght
    }

    private void FreeStateUpdate()
    {
        //decrease start=>end 
        //position start=_prev.End
        //Rotate around Start  lookat end
    }

    public void Init(RopeSegment nextSegment, Vector3 startPointCoordinate, float thickness, Rope rope)
    {
        _startPointCoordinate = startPointCoordinate;
        _thickness = thickness;
        ChangeNextSegment(nextSegment);
        //BarrierTouched += rope.SegmentDivision;
        //ReachedDeleatingState += rope.DeleteSegment;

        UpdateState();
    }

    public void UpdateState()
    {
        RefreshPosition();
        RefreshRotation();
        RefreshLenght();
        
    }

    private void RefreshPosition()
    {
        transform.position = new Vector3(_startPointCoordinate.x, transform.position.y, _startPointCoordinate.z);
    }

    private void RefreshLenght()
    {
        float lenght = Vector3.Distance(transform.position, _nextSegment.StartPointCoordinate);

        if (lenght > _minLenght)
        {
            _lenght = lenght;
        }
        else
        {
            _lenght = _minLenght;

            if (!_isBarrierTouch && !_isStartPointLock && !_isEndPointLock)
            {
                //ReachedDeleatingState?.Invoke(this);
            }
        }

        transform.localScale = new Vector3(_thickness, _lenght, _thickness);
    }

    private void RefreshRotation()
    {
        _angle = Vector3.Angle(Vector3.forward, _nextSegment.StartPointCoordinate - _startPointCoordinate);
        float sign = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(Vector3.forward, _nextSegment.StartPointCoordinate - _startPointCoordinate)));
        transform.eulerAngles = new Vector3(90,  _angle*sign, 0);
        //Debug.Log(angle + " ");
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
