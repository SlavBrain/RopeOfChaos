using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(LineDrawer))]
public class RopeCreator : MonoBehaviour
{
    [SerializeField] private GameObject _ropeTemplate;
    [SerializeField] private float _wight = 0.2f;
    private LineRenderer _lineRenderer;
    [SerializeField] private List<GameObject> _ropeSegments;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
    private void OnEnable()
    {
        if (_lineRenderer.positionCount > 3)
            CreateRope();
        else
            Debug.LogError("No dots for create Rope");
    }
    private void OnDisable()
    {
        ClearRope();
    }

    private void CreateRope()
    {
        for (int i = 0; i < _lineRenderer.positionCount - 1; i++)
        {
            CreateRopeSegment(_lineRenderer.GetPosition(i), _lineRenderer.GetPosition(i + 1));
        }

        CreateRopeSegment(_lineRenderer.GetPosition(_lineRenderer.positionCount - 1), _lineRenderer.GetPosition(0));
    }

    private void CreateRopeSegment(Vector3 startPoint, Vector3 endPoint)
    {
        GameObject segment = Instantiate(_ropeTemplate, startPoint, Quaternion.identity, transform);
        segment.transform.localScale = new Vector3(_wight, Vector3.Distance(startPoint, endPoint), _wight);
        segment.transform.LookAt(endPoint, Vector3.up);
        segment.transform.Rotate(90, 0, 0);
        _ropeSegments.Add(segment);
    }

    private void ClearRope()
    {
        for (int i = 0; i < _ropeSegments.Count; i++)
        {
            Destroy(_ropeSegments[i]);
        }

        _ropeSegments.Clear();
    }
}
