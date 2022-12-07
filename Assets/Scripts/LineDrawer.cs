using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] private float _width;
    [SerializeField] private float _deltaLenght;
    [SerializeField] private float _maxDistanseForLoop = 1;

    private LineRenderer _lineRenderer;

    public UnityEvent DrawedLoop;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = _width;
        _lineRenderer.endWidth = _width;
    }

    private void OnEnable()
    {
        _lineRenderer.loop = false;
        _lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && _lineRenderer.positionCount > 0)
        {
            DrawLine();
        }

        if (Input.GetMouseButtonDown(0))
        {
            _lineRenderer.positionCount = 1;

            if(TryGetWorldCoordinate(Input.mousePosition, out Vector3 currentPosition))
            {
                _lineRenderer.SetPosition(0, currentPosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!TryLoop())
            {
                _lineRenderer.positionCount = 0;
            }
        }


    }

    private void DrawLine()
    {
        if(TryGetWorldCoordinate(Input.mousePosition, out Vector3 currentPosition))
        {
            if (Vector3.Distance(_lineRenderer.GetPosition(_lineRenderer.positionCount-1), currentPosition) > _deltaLenght)
            {
                _lineRenderer.positionCount++;
            }

            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, currentPosition);
        }
    }

    private bool TryLoop()
    {
        if(Vector3.Distance(_lineRenderer.GetPosition(_lineRenderer.positionCount - 1), _lineRenderer.GetPosition(0)) < _maxDistanseForLoop)
        {
            _lineRenderer.loop = true;
            _lineRenderer.Simplify(0.3f);
            DrawedLoop?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool TryGetWorldCoordinate(Vector3 touchPosition, out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out RaycastHit hitData, 1000) && hitData.transform.TryGetComponent<Ground>(out Ground ground))
        {
            worldPoint = hitData.point + new Vector3(0, 0.5f, 0);
            return true;
        }
        else
        {
            return false;
        }
    }
}
