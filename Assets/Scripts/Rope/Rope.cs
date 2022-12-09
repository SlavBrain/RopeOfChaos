using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private List<RopeSegment> _segments;
    [SerializeField] private int _minSegmentsCount = 50;
    [SerializeField] private GameObject _ropeTemplate;
    [SerializeField] private float _thickness = 0.1f;
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
            newSegment.Init(dots[i], _segments[i - 1], _thickness,this);
        }


        firstNewSegment.Init(dots[0], _segments[_segments.Count - 1], _thickness,this);
    }

    public void SegmentDivision(RopeSegment oldSegment)
    {
        Vector3 breakPoint = (oldSegment.StartPoint + oldSegment.PreviousSegment.StartPoint) / 2;
        RopeSegment newSegment = AddNewSegment();
        newSegment.Init(breakPoint, oldSegment.PreviousSegment, _thickness,this);
        oldSegment.Init(oldSegment.StartPoint, newSegment, _thickness,this);

        Debug.Log("division");
    }

    public void DeleteSegment(RopeSegment segment)
    {
        if (_segments.Count > _minSegmentsCount)
        {
            Debug.Log("delete");
            segment.NextSegment.ChangePreviousSegment(segment.PreviousSegment);
            //segment.PreviousSegment.ChangeNextSegment(segment.NextSegment);
            Destroy(segment.gameObject);
            //segment.gameObject.SetActive(false);
        }        
    }

    private RopeSegment AddNewSegment()
    {
        RopeSegment newSegment = Instantiate(_ropeTemplate, transform).AddComponent<RopeSegment>();
        _segments.Add(newSegment);
        return newSegment;
    }
}
