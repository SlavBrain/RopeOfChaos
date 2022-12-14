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
        RopeSegment firstNewSegment = Instantiate(_ropeTemplate, dots[0],Quaternion.identity,transform).AddComponent<RopeSegment>();
        _segments.Add(firstNewSegment);

        for (int i = 1; i < dots.Length; i++)
        {
            RopeSegment newSegment = Instantiate(_ropeTemplate,dots[i],Quaternion.identity, transform).AddComponent<RopeSegment>();
            _segments.Add(newSegment);
            newSegment.Init( _segments[i - 1],  _thickness,this);
        }


        firstNewSegment.Init( _segments[_segments.Count - 1], _thickness,this);
    }

    public void SegmentDivision(RopeSegment oldSegment)
    {
        Vector3 breakPoint = (oldSegment.transform.position + oldSegment.EndPoint.transform.position) / 2;
        RopeSegment newSegment = Instantiate(_ropeTemplate, breakPoint, Quaternion.identity, transform).AddComponent<RopeSegment>();
        _segments.Add(newSegment);
        newSegment.Init( oldSegment.NextSegment, _thickness,this);
        oldSegment.Init( newSegment, _thickness,this);

        Debug.Log("division");
    }

    public void DeleteSegment(RopeSegment segment)
    {
        if (_segments.Count > _minSegmentsCount)
        {
            Debug.Log("delete");
            segment.PreviousSegment.ChangeNextSegment(segment.NextSegment);
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
