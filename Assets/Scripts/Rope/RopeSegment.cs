using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    [SerializeField] private const float _minLenght = 0.1f;
    [SerializeField] private Vector3 _startPoint;
    [SerializeField] private RopeSegment _nextSegment;
    [SerializeField] private float _lenght;
    [SerializeField] private float _thickness;
    [SerializeField] private Collider _collider;
    [SerializeField] private bool _isBarrierTouch;

    public Vector3 StartPoint => _startPoint;
    private void Awake()
    {
        _collider = GetComponentInChildren<Collider>();
    }
    private void Update()
    {
        Decrease();
        UpdateState();
    }

    private void Decrease()
    {
        if (!_isBarrierTouch)
        {
            _startPoint = Vector3.MoveTowards(_startPoint, _nextSegment.StartPoint, Time.deltaTime * 10f);
        }
    }

    public void Init(Vector3 startPoint, RopeSegment nextSegment, float thickness)
    {
        _startPoint = startPoint;
        _nextSegment = nextSegment;
        _thickness = thickness;

        UpdateState();
    }

    public void UpdateState()
    {
        RefreshPosition();
        RefreshLenght();
        RefreshRotation();
    }

    private void RefreshPosition()
    {
        transform.position = _startPoint;
    }

    private void RefreshLenght()
    {
        float lenght = Vector3.Distance(_startPoint, _nextSegment.StartPoint);

        if (lenght > _minLenght)
        {
            _lenght = lenght;
        }
        else
        {
            _lenght = _minLenght;
        }

        transform.localScale = new Vector3(_thickness, _lenght, _thickness);
    }

    private void RefreshRotation()
    {
        transform.LookAt(_nextSegment.StartPoint, Vector3.up);
        transform.Rotate(90, 0, 0);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.TryGetComponent<Barrier>(out Barrier barrier))
        {
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
