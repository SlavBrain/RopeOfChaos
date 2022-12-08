using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField]private List<RopeSegment> _segments;
    [SerializeField]private GameObject _ropeTemplate;
    [SerializeField] private float _thickness=0.1f;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Vector3[] _dots;
    
    private void OnEnable()
    {
        _dots = new Vector3[_lineRenderer.positionCount];
        _lineRenderer.GetPositions(_dots);
           Init(_dots);
    }

    public void Init(Vector3[] dots)
    {
        RopeSegment firstNewSegment = Instantiate(_ropeTemplate, transform).AddComponent<RopeSegment>();
        _segments.Add(firstNewSegment);

        for (int i = 1; i < dots.Length; i++)
        {
                RopeSegment newSegment = Instantiate(_ropeTemplate, transform).AddComponent<RopeSegment>();
                _segments.Add(newSegment); 
                newSegment.Init(dots[i], _segments[i-1], _thickness);
        }

        
        firstNewSegment.Init(dots[0], _segments[_segments.Count - 1], _thickness);
    }
}
