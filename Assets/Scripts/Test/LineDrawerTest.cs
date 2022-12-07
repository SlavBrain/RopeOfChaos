using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineDrawerTest : MonoBehaviour
{
    private const int _numberPointForSmothess = 4;
    private const int _numberDotsInRopeSegment = 2;

    [SerializeField] private GameObject _ground;
    [SerializeField] private float _width;
    [SerializeField] private float _deltaLenght;
    [SerializeField] private float _maxDistanseForLoop = 1;

    [SerializeField] private float _decreaseSpeed = 1;
    private LineRenderer _lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    private Coroutine decrease;
    [SerializeField] private List<BoxCollider> ropeColliders;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = _width;
        _lineRenderer.endWidth = _width;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (TryGetWorldCoordinate(Input.mousePosition, out Vector3 currentPosition))
            {
                if (points.Count != 0)
                {
                    if (Vector3.Distance(points[points.Count - 1], currentPosition) > _deltaLenght)
                    {
                        points.Add(currentPosition);
                        _lineRenderer.positionCount += _numberDotsInRopeSegment;

                        if (points.Count > _numberPointForSmothess)
                        {
                            DrawCubicBezierCurve(points[points.Count - 4], points[points.Count - 3], points[points.Count - 2], points[points.Count - 1]);
                        }
                    }
                    else
                    {
                        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, currentPosition);
                    }
                }
                else
                {
                    points.Add(currentPosition);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && points.Count > 0)
        {
            if (Vector3.Distance(_lineRenderer.GetPosition(0), _lineRenderer.GetPosition(_lineRenderer.positionCount - 1)) < _maxDistanseForLoop)
            {
                _lineRenderer.loop = true;
                _lineRenderer.Simplify(0.001f);
                CreateColliders();
                DecreaseLoop();
            }
            else
            {
                ClearRope();
            }
        }

    }

    private void CreateColliders()
    {
        for (int i = 0; i <= _lineRenderer.positionCount - 1; i++)
        {
            BoxCollider collider = new GameObject("num " + i).AddComponent<BoxCollider>();
            ropeColliders.Add(collider);
            collider.transform.parent = _lineRenderer.transform;

            if (i == _lineRenderer.positionCount - 1)
            {
                RefreshColliders(i, _lineRenderer.GetPosition(i), _lineRenderer.GetPosition(0));
            }
            else
            {
                RefreshColliders(i);
            }
        }


    }

    private void RefreshColliders(int number)
    {
        Vector3 startPoint = _lineRenderer.GetPosition(number);
        Vector3 endPoint = _lineRenderer.GetPosition(number + 1);

        float lineLenght = Vector3.Distance(startPoint, endPoint);
        ropeColliders[number].size = new Vector3(lineLenght, _width, _width);

        Vector3 midPoint = (startPoint + endPoint) / 2;
        ropeColliders[number].transform.position = midPoint;

        float angle = (Mathf.Abs(startPoint.z + endPoint.z) / Mathf.Abs(startPoint.x + endPoint.x));
        if ((startPoint.z < endPoint.z && startPoint.x > endPoint.z) || (startPoint.z > endPoint.z && startPoint.x < endPoint.z))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        //float angle = Vector3.Angle(_lineRenderer.GetPosition(number), _lineRenderer.GetPosition(number - 1));
        ropeColliders[number].transform.Rotate(0, angle, 0);
    }

    private void RefreshColliders(int number, Vector3 startPoint, Vector3 endPoint)
    {
        float lineLenght = Vector3.Distance(startPoint, endPoint);
        ropeColliders[number].size = new Vector3(lineLenght, _width, _width);

        Vector3 midPoint = (startPoint + endPoint) / 2;
        ropeColliders[number].transform.position = midPoint;

        float angle = (Mathf.Abs(startPoint.z + endPoint.z) / Mathf.Abs(startPoint.x + endPoint.x));
        if ((startPoint.z < endPoint.z && startPoint.x > endPoint.z) || (startPoint.z > endPoint.z && startPoint.x < endPoint.z))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        //float angle = Vector3.Angle(_lineRenderer.GetPosition(number), _lineRenderer.GetPosition(number - 1));
        ropeColliders[number].transform.Rotate(0, angle, 0);
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

    private void DrawCubicBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3)
    {
        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = _lineRenderer.positionCount - 4 * _numberDotsInRopeSegment; i < _lineRenderer.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * (1 - t) * point0 + 3 * (1 - t) * (1 - t) *
                t * point1 + 3 * (1 - t) * t * t * point2 + t * t * t * point3;

            _lineRenderer.SetPosition(i, B);
            t += (1 / (float)_lineRenderer.positionCount);
        }
    }

    private void DecreaseLoop()
    {
        if (decrease != null)
        {
            StopCoroutine(decrease);
        }

        decrease = StartCoroutine(Decreasing());
    }

    private void ClearRope()
    {
        _lineRenderer.positionCount = 0;
        points.Clear();
        ClearColliders();
        _lineRenderer.loop = false;
    }

    private void ClearColliders()
    {
        for(int i = 0; i < ropeColliders.Count; i++)
        {
            Destroy(ropeColliders[i].gameObject);
        }

        ropeColliders.Clear();
    }

    private IEnumerator Decreasing()
    {
        var elapsedTime = 0f;
        while (elapsedTime < 5)
        {
            float minDistance = _decreaseSpeed / _lineRenderer.positionCount * 0.1f;//сделать зависимость между минимальным радиусом и количеством точек окружности
            for (int i = 0; i < _lineRenderer.positionCount - 1; i++)
            {
                Debug.Log(i);
                if (Vector3.Distance(_lineRenderer.GetPosition(i), _lineRenderer.GetPosition(i + 1)) > minDistance + 0.01f)
                {
                    Vector3 newPointPosition = Vector3.MoveTowards(_lineRenderer.GetPosition(i), _lineRenderer.GetPosition(i + 1), minDistance);
                    _lineRenderer.SetPosition(i, newPointPosition);
                    RefreshColliders(i);
                }
            }

            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, Vector3.MoveTowards(_lineRenderer.GetPosition(_lineRenderer.positionCount - 1), _lineRenderer.GetPosition(0), minDistance));
            Debug.Log("last");

            RefreshColliders(_lineRenderer.positionCount - 1, _lineRenderer.GetPosition(_lineRenderer.positionCount - 1), _lineRenderer.GetPosition(0));

            //_lineRenderer.Simplify(0.001f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ClearRope();
    }
}
